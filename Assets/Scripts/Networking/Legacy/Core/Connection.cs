using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

#pragma warning disable CS0618 // Type or member is obsolete
public abstract class ConnectionBase : MonoBehaviour
{
	public enum Channel { Reliable, Unreliable, FileTransfer };
	public int connectionHostId { private set; get; }

	/// <summary>
	/// Happens right after connection is estabilished. No ClientData is filled at this point.
	/// </summary>
	public Action<int> onConnected;
	/// <summary>
	/// Might be lost in out of focus cases.
	/// </summary>
	public Action<int> onDisconnected;

	protected readonly int broadcastPort = 8888;
	protected readonly int broadcastKey = 6496;
	protected readonly int connectPort = 8880;
	protected readonly int bufferSize = 2048;

	public bool IsReady;

	static OrderHandlerManager dataReceivedHandler;

	protected virtual void Update()
	{
		if (!IsReady)
		{
			return;
		}

		int outHostId;
		int outConnectionId;
		int outChannelId;
		byte[] buffer = new byte[bufferSize];
		int receivedSize;
		byte error;
		string outIp;
		int outPort;
		NetworkID outNetwork;
		NodeID outDstNode;
		NetworkEventType eventType = NetworkEventType.Nothing;
		do
		{
			eventType = NetworkTransport.Receive(out outHostId, out outConnectionId, out outChannelId, buffer, buffer.Length, out receivedSize, out error);
			switch (eventType)
			{
				case NetworkEventType.ConnectEvent:
					NetworkTransport.GetConnectionInfo(outHostId, outConnectionId, out outIp, out outPort, out outNetwork, out outDstNode, out error);
					DebugNet.Log(string.Format("Connected on {0} id.", outConnectionId));
					ConnectEvent(outConnectionId, outIp);
					break;
				case NetworkEventType.DisconnectEvent:
					DebugNet.Log(string.Format("Disconnected on {0} id.", outConnectionId));
					DisconnectEvent(outConnectionId);
					break;
				case NetworkEventType.DataEvent:
					DataEvent(outConnectionId, buffer);
					break;

			}
		} while (eventType != NetworkEventType.Nothing);
	}

	protected virtual void ConnectEvent(int outConnectionId, string otherIp)
	{
		onConnected?.Invoke(outConnectionId);
	}
	protected virtual void DisconnectEvent(int outConnectionId)
	{
		onDisconnected?.Invoke(outConnectionId);
	}

	protected virtual void HandleDataEvent(int connectionId, Order order, string data)
	{
		DebugNet.Log(string.Format("Got order {0} with data {1}.",
			order, data));
		dataReceivedHandler.ParseData(connectionId, order, data);
	}

	void DataEvent(int connectionId, byte[] buffer)
	{
		string bufferString = Serializator.GetObject<string>(buffer);
		string data;
		Order order = OrderParser.ParseMessage(bufferString, out data);
		HandleDataEvent(connectionId, order, data);
	}

	public abstract NetworkError Send(Order order, Channel channel, OrderData data);

	public virtual NetworkError Send(int connectionId, Order order, Channel channel, OrderData data)
	{
		byte error;
		string jsonData = JsonUtility.ToJson(data);
		if (!IsReady)
		{
			DebugNet.Log(string.Format("Cannot send data without connection!. Order: {0} with data: {1}", order, jsonData));
			return NetworkError.WrongOperation;
		}
		byte[] byteData = OrderParser.CreateMessage(order, jsonData);
		NetworkTransport.Send(connectionHostId, connectionId, (int)channel, byteData, byteData.Length, out error);
		DebugNet.Log(string.Format("Sent order {0} with data {1}.", order, jsonData));
		return (NetworkError)error;
	}

	protected int OpenHost(int? port = null)
	{
		ConnectionConfig config = new ConnectionConfig();
		config.DisconnectTimeout = 5000;
		config.AddChannel(QosType.ReliableSequenced);
		config.AddChannel(QosType.Unreliable);
		config.AddChannel(QosType.ReliableFragmentedSequenced);
		HostTopology topology = new HostTopology(config, 32);
		int hostId = -1;
		if (port != null)
		{
			hostId = NetworkTransport.AddHost(topology, port.GetValueOrDefault());
		}
		else
		{
			hostId = NetworkTransport.AddHost(topology);
		}
		DebugNet.Log("Opened host " + hostId);
		return hostId;
	}

	protected void CloseHost(int id)
	{
		NetworkTransport.RemoveHost(id);
		DebugNet.Log("Closed host " + id);
	}

	protected virtual void InitNewInstance()
	{
		if (!NetworkTransport.IsStarted)
		{
			NetworkTransport.Init();
		}
		connectionHostId = OpenHost(connectPort);
		dataReceivedHandler = new OrderHandlerManager();
	}

	protected virtual void OnDestroy()
	{
		CloseHost(connectionHostId);
		if (NetworkTransport.IsStarted)
		{
			NetworkTransport.Shutdown();
		}
	}

	#region SINGLETON
	public static bool instanceExists {
		get {
			return baseInstance != null;
		}
	}

	public static ConnectionBase baseInstance {
		private set {
			if (value == null || _baseInstance != null)
			{
				Debug.LogError("Something went terribly wrong.");
			}
			if (_baseInstance != value)
			{
				_baseInstance = value;
				_baseInstance.InitNewInstance();
				if (_baseInstance.GetComponent<DontDestroyThis>() == null
					&& Application.isPlaying)
				{
					_baseInstance.gameObject.AddComponent<DontDestroyThis>();
				}
			}
		}
		get {
			if (_baseInstance == null)
			{
				return FindObjectOfType<ConnectionBase>();
			}
			return _baseInstance;
		}
	}
	static ConnectionBase _baseInstance;

	protected static T GetInstance<T, U>() where T : ConnectionBase where U : ConnectionBase
	{
		if (baseInstance != null && baseInstance as T != null)
		{
			return baseInstance as T;
		}
		T instanceToSet = FindObjectOfType<T>();
		if (instanceToSet != null)
		{
			baseInstance = instanceToSet;
			return baseInstance as T;
		}
		U otherClass = FindObjectOfType<U>();
		if (otherClass == null)
		{
			CreateNewInstance<T>();
		}
		else
		{
			Debug.LogError("Cannot create " + typeof(T).Name + " since "
				+ typeof(U).Name + " exists.", otherClass);
		}
		return baseInstance as T;
	}

	protected static void CreateNewInstance<T>() where T : ConnectionBase
	{
		GameObject newGO = new GameObject(typeof(T).Name);
		newGO.AddComponent<T>();
	}

	void Awake()
	{
		if (_baseInstance != null && _baseInstance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else if (_baseInstance == this)
		{
			return;
		}
		else
		{
			baseInstance = this;
		}
	}
	#endregion
}
#pragma warning restore CS0618 // Type or member is obsolete
