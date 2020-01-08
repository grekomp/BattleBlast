using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(IntroduceSender))]
[RequireComponent(typeof(IntroduceResponseHandler))]
[RequireComponent(typeof(IntroduceConfirmSender))]
public class ConnectionServer : Connection {
	#region CLIENT_MANAGEMENT
	public event Action<ClientData> onClientDataUpdate;
	/// <summary>
	/// Happens after introductory exchange. Later than Connection.ConnectEvent
	/// </summary>
	public event Action<ClientData> onClientConnected;
	/// <summary>
	/// Happens only if introductory exchange was done. It is skipped if client disconnects beforehand.
	/// </summary>
	public event Action<ClientData> onClientDisconnected;
	/// <summary>
	/// Happens after either onClientConnected, onClientDisconnected or onClientDataUpdate.
	/// </summary>
	public event Action onClientListUpdate;
	public int clientCount {
		get {
			return clientManager.clients.Count;
		}
	}
	[SerializeField] ClientManager clientManager;

	public ClientData ClientFromConnectionId(int id) {
		return clientManager.clients.Find(x => x.connectionId == id);
	}

	public ClientData ClientFromDeviceId(string id) {
		return clientManager.clients.Find(x => x.deviceId == id);
	}

	public List<ClientData> GetConnectedClients() { return new List<ClientData>(clientManager.clients); }
	#endregion

	int broadcastHostId;

	IntroduceSender introduceSender;
	IntroduceConfirmSender introduceConfirmSender;

	protected override void ConnectEvent(int outConnectionId, string otherIp) {
		clientManager.OnClientConnected(outConnectionId, otherIp);
		introduceSender.SendIntroduction(outConnectionId);
	}

	protected override void DisconnectEvent(int outConnectionId) {
		base.DisconnectEvent(outConnectionId);
		ClientData data = clientManager.OnClientDisconnected(outConnectionId);
		if (data != null) {
			DebugNet.Log(string.Format("Lost {0}.", data.ToString()));
			onClientDisconnected?.Invoke(data);
			onClientListUpdate?.Invoke();
		}
	}

	protected override void HandleDataEvent(int connectionId, Order order, string data) {
		base.HandleDataEvent(connectionId, order, data);
	}

	protected override void InitNewInstance() {
		base.InitNewInstance();
		introduceSender = GetComponent<IntroduceSender>();
		introduceConfirmSender = GetComponent<IntroduceConfirmSender>();
		StartBroadcast();
		clientManager = new ClientManager();
		clientManager.Init();
		clientManager.onAddClientData += FireOnClientConnected;
		IntroduceResponseHandler.onIntroduceResponse += (x) => {
			introduceConfirmSender.Send(x.clientId);
		};
	}

	void FireOnClientConnected(ClientData client) {
		base.ConnectEvent(client.connectionId, client.ip);
		onClientConnected?.Invoke(client);
		onClientListUpdate?.Invoke();
	}

	void StartBroadcast() {
		if (!NetworkTransport.IsBroadcastDiscoveryRunning()) {
			broadcastHostId = OpenHost(0);
			byte error;
			byte[] buffor = Serializator.GetBytes(SystemInfo.deviceName);
			NetworkTransport.StartBroadcastDiscovery(broadcastHostId, broadcastPort, broadcastKey, 1, 1, buffor, buffor.Length, 1000, out error);
			IsReady = true;
			DebugNet.Log("Broadcasting: " + (NetworkError)error);
		}
	}

	void StopBroadcast() {
		NetworkTransport.StopBroadcastDiscovery();
		DebugNet.Log("Stopped broadcasting.");
		CloseHost(broadcastHostId);
	}

	protected override void OnDestroy() {
		StopBroadcast();
		base.OnDestroy();
	}

	public override NetworkError Send(Order order, Channel channel, OrderData data) {
		NetworkError toReturn = NetworkError.Ok;
		List<ClientData> clients = clientManager.clients;
		foreach (ClientData client in clients) {
			data.clientId = client.clientId;
			NetworkError error = Send(client.connectionId, order, channel, data);
			if (error != NetworkError.Ok) {
				toReturn = error;
			}
		}
		return toReturn;
	}

	public override NetworkError Send(int connectionId, Order order, Channel channel, OrderData data) {
		data.clientId = connectionId;
		return base.Send(connectionId, order, channel, data);
	}

	#region SINGLETON
	public static ConnectionServer instance {
		get {
			return GetInstance<ConnectionServer, ConnectionClient>();
		}
	}
	#endregion
}
