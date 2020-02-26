using Athanor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Networking
{
	public class NetServer : ScriptableObject
	{
		public enum Status
		{
			Uninitialized,
			InitializedNotRunning,
			Running,
			Stopped,
			Error
		}

		[Header("Runtime Variables")]
		public NetHost netHost;
		[SerializeField] protected NetServerClientManager clientManager = new NetServerClientManager();
		[SerializeField] [Disabled] protected Status status = Status.Uninitialized;

		#region Raised events
		[Header("Raised Events")]
		public GameEventHandler OnDataReceived;

		public GameEventHandler OnClientConnected;
		public GameEventHandler OnClientDisconnected;

		public event Action<NetReceivedData> onDataReceived;

		public event Action<ConnectedClient> onClientConnected;
		public event Action<ConnectedClient> onClientDisconnected;
		#endregion


		#region Public properties
		public NetServerClientManager ClientManager => clientManager;
		public Status ServerStatus => status;
		#endregion


		#region Initialization
		protected NetServer() { }
		public static NetServer CreateServer(NetHost host = null)
		{
			NetServer instance = CreateInstance<NetServer>();

			if (host == null)
			{
				host = NetCore.Instance.AddHost(hostName: "NetServer Host");
			}

			instance.Initialize(host);
			return instance;
		}
		protected void Initialize(NetHost host)
		{
			if (status != Status.Uninitialized)
			{
				Log.WTF(this, $"Cannot initialize NetServer, because the status is {status.ToString()}", this);
				return;
			}

			netHost = host;

			// Bind events
			netHost.OnConnectEvent.RegisterListenerOnce(HandleClientConnected);
			netHost.OnDisconnectEvent.RegisterListener(HandleClientDisconnected);
			netHost.OnDataEvent.RegisterListener(HandleDataReceived);

			// Set status
			status = Status.InitializedNotRunning;
		}
		#endregion


		#region Starting and stopping server
		public void Start()
		{
			// Start broadcasting server port
			NetCore.Instance.StartBroadcastDiscovery(netHost.Port);

			// Set correct status
			status = Status.Running;
		}
		public void Stop()
		{
			// Stop broadcasting
			NetCore.Instance.StopBroadcastDiscovery();

			// Set correct status
			status = Status.Stopped;
		}
		#endregion


		#region Event handling
		public void HandleDataReceived(GameEventData gameEventData)
		{
			if (gameEventData.data is NetReceivedData data)
			{
				OnDataReceived.Raise(this, data);
				onDataReceived?.Invoke(data);
			}
		}
		public void HandleClientConnected(GameEventData gameEventData)
		{
			if (gameEventData.data is NetConnection connection)
			{
				ConnectedClient connectedClient = clientManager.NewClientConnected(connection);
				OnClientConnected.Raise(this, connectedClient);
				onClientConnected?.Invoke(connectedClient);
			}
		}
		public void HandleClientDisconnected(GameEventData gameEventData)
		{
			if (gameEventData.data is NetConnection connection)
			{
				ConnectedClient client = clientManager.ClientDisconnected(connection);
				OnClientDisconnected.Raise(this, client);
				onClientDisconnected?.Invoke(client);
			}
		}
		#endregion
	}
}
