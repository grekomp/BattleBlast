using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

#pragma warning disable CS0618 // Type or member is obsolete
[RequireComponent(typeof(IntroduceHandler))]
[RequireComponent(typeof(IntroduceResponseSender))]
[RequireComponent(typeof(IntroduceConfirmHandler))]
public class ConnectionClient : ConnectionBase
{
	public Action<ServerData> onConnectedToServer;
	public Action<ServerData> onDisconnectedFromServer;

	public enum Status { None, Scanning, Connecting, Introducing, Connected }
	public Status status { private set; get; }

	public int clientId;

	int scanningHostId;
	ServerData serverData;
	int connectionId;
	string serverIp;

	protected override void OnDestroy()
	{
		StopScanning();
		base.OnDestroy();
	}

	protected override void Update()
	{
		int outHostId;
		int outConnectionId;
		int outChannelId;
		byte[] buffer = new byte[bufferSize];
		int receiveSize;
		byte error;

		switch (status)
		{
			case Status.None:
				StartScanning();
				break;
			case Status.Scanning:
				NetworkEventType scanningEvent = NetworkTransport.ReceiveFromHost(scanningHostId, out outConnectionId, out outChannelId, buffer, bufferSize, out receiveSize, out error);
				if (scanningEvent != NetworkEventType.Nothing)
				{
					DebugNet.Log(string.Format("Received: " + scanningEvent));
				}
				if (scanningEvent == NetworkEventType.BroadcastEvent)
				{
					string senderAddress;
					int senderPort;
					byte broadcastError;
					int receivedSize;
					NetworkTransport.GetBroadcastConnectionMessage(scanningHostId, buffer, bufferSize, out receivedSize, out error);
					NetworkTransport.GetBroadcastConnectionInfo(scanningHostId, out senderAddress, out senderPort, out broadcastError);
					if (broadcastError == (int)NetworkError.Ok && error == (int)NetworkError.Ok)
					{
						DebugNet.Log("Found server.");
						serverIp = senderAddress;
						serverData = new ServerData(0, serverIp.Substring(7), Serializator.GetObject<string>(buffer));
						StartConnectingToServer(senderAddress);
					}
				}
				break;
			case Status.Connecting:
				NetworkEventType connectingEvent = NetworkEventType.Nothing;
				do
				{
					connectingEvent = NetworkTransport.Receive(out outHostId, out outConnectionId, out outChannelId, buffer, bufferSize, out receiveSize, out error);
					if (connectingEvent != NetworkEventType.Nothing)
					{
						DebugNet.Log(string.Format("Received: " + connectingEvent));
					}

					switch (connectingEvent)
					{
						case NetworkEventType.ConnectEvent:
							if ((NetworkError)error == NetworkError.Ok)
							{
								StopScanning();
								status = Status.Introducing;
								IsReady = true;
								return;
								//Connected event is fired after confirmation from server in Introduce
							}
							break;
						case NetworkEventType.DisconnectEvent:
							DebugNet.Log("Disconnect during connecting:" + (NetworkError)error);
							status = Status.Scanning;
							DisconnectEvent(connectionId);
							break;
					}
					if ((NetworkError)error != NetworkError.Ok)
					{
						DebugNet.Log("Connection error:" + (NetworkError)error);
						status = Status.Scanning;
					}
				} while (connectingEvent != NetworkEventType.Nothing);
				break;
		}
		base.Update();
	}

	protected override void ConnectEvent(int connectionId, string otherIp)
	{
		status = Status.Connected;
		base.ConnectEvent(connectionId, otherIp);
		DebugNet.Log("Connected to server.");
		serverData.connectionId = connectionId;
		onConnectedToServer?.Invoke(serverData);
	}

	protected override void DisconnectEvent(int outConnectionId)
	{
		status = Status.None;
		DebugNet.Log("Lost server " + outConnectionId);
		if (IsReady)
		{
			IsReady = false;
			base.DisconnectEvent(outConnectionId);
			onDisconnectedFromServer?.Invoke(serverData);
		}
	}

	protected override void HandleDataEvent(int connectionId, Order order, string data)
	{
		base.HandleDataEvent(connectionId, order, data);
	}

	protected override void InitNewInstance()
	{
		base.InitNewInstance();
		IntroduceConfirmHandler.introductionFinished += HandleOnIntroduce;
	}

	public ServerData GetServerData()
	{
		return serverData;
	}

	void HandleOnIntroduce(OrderData data)
	{
		ConnectEvent(connectionId, serverIp);
	}

	void StartScanning()
	{
		scanningHostId = OpenHost(broadcastPort);
		byte error;
		NetworkTransport.SetBroadcastCredentials(scanningHostId, broadcastKey, 1, 1, out error);
		if (error == (int)NetworkError.Ok)
		{
			DebugNet.Log("Scanning for server started!");
			status = Status.Scanning;
		}
	}

	void StopScanning()
	{
		if (scanningHostId >= 0)
		{
			CloseHost(scanningHostId);
			scanningHostId = -1;
		}
	}

	void StartConnectingToServer(string serverIP)
	{
		byte error;
		DebugNet.Log("Connecting to server at: " + serverIP);
		connectionId = NetworkTransport.Connect(connectionHostId, serverIP,
			connectPort, 0, out error);
		if (error == (int)NetworkError.Ok)
		{
			status = Status.Connecting;
		}
		else
		{
			DebugNet.Log("Connection error: " + (NetworkError)error);
		}
	}

	public override NetworkError Send(Order order, Channel channel, OrderData data)
	{
		data.clientId = clientId;
		return Send(connectionId, order, channel, data);
	}

	#region SINGLETON
	public static ConnectionClient instance {
		get {
			return GetInstance<ConnectionClient, ConnectionServer>();
		}
	}
	#endregion
}
#pragma warning restore CS0618 // Type or member is obsolete
