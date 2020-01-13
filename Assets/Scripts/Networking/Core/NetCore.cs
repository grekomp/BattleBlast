using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using Utils;

#pragma warning disable CS0618 // Type or member is obsolete
namespace Networking
{
	/// <summary>
	/// Low-level core networking class, abstracting the actual network implementation.
	/// </summary>
	public class NetCore : DontDestroySingleton<NetCore>
	{
		public static readonly string LogTag = "Networking";

		#region Options
		[Serializable]
		public class Options
		{
			public int broadcastPort = 8890;
			public int broadcastKey = 8671;
			public int broadcastVersion = 1;
			public int broadcastSubversion = 1;
			[Space]
			public int maxConnections = 10;
		}

		[Header("Networking Core Options")]
		[SerializeField]
		protected Options options = new Options();
		public ConnectionConfigReference defaultConnectionConfig;

		public int BroadcastPort { get => options.broadcastPort; }
		#endregion

		[Header("Runtime Variables")]
		[SerializeField] protected NetHost broadcastHost;
		[SerializeField] protected NetHost scanningHost;
		[Space]
		[SerializeField] protected List<NetHost> activeHosts = new List<NetHost>();

		private bool IsBroadcasting => NetworkTransport.IsBroadcastDiscoveryRunning();
		private bool IsScanningForBroadcast => scanningHost.Id >= 0;
		public bool IsInitialized { get; private set; }

		[Header("Events")]
		public GameEventHandler OnConnectEvent = new GameEventHandler();
		public GameEventHandler OnDisconnectEvent = new GameEventHandler();
		public GameEventHandler OnDataReceivedEvent = new GameEventHandler();
		public GameEventHandler OnBroadcastEvent = new GameEventHandler();

		#region Initialization
		public void Awake() => Initialize();
		[ContextMenu("Initialize")]
		public void Initialize()
		{
			if (IsInitialized) return;

			Log.Info(LogTag, $"Initializing networking core: {this}.", this);
			if (NetworkTransport.IsStarted == false)
			{
				NetworkTransport.Init();
				Log.Info(LogTag, "Initialized NetworkTransport.", this);
			}

			broadcastHost = NetHost.Null;
			scanningHost = NetHost.Null;

			IsInitialized = true;
		}
		#endregion

		#region Sending Data
		public virtual NetworkError Send(int hostId, int connectionId, int channel, byte[] data)
		{
			if (InitCheck() == false) return NetworkError.WrongOperation;

			NetworkTransport.Send(hostId, connectionId, channel, data, data.Length, out byte error);

			NetworkError networkError = (NetworkError)error;
			if (networkError == NetworkError.Ok)
			{
				Log.Verbose(LogTag, $"Sent data via: HostId: {hostId}, ConnectionId: {connectionId}, Channel: {channel}. \nData: {data}.", this);
			}
			else
			{
				Log.Warning(LogTag, $"Failed to send data with error: {networkError} via HostId: {hostId}, ConnectionId: {connectionId}, Channel: {channel}. \nData: {data}.", this);
			}
			return networkError;
		}
		#endregion

		#region Handling Incoming Events
		[ContextMenu("Update")]
		public void Update()
		{
			if (IsInitialized == false) return;

			byte[] buffer = new byte[2048];
			NetworkEventType eventType;
			do
			{
				eventType = NetworkTransport.Receive(out int receivedHostId, out int receivedConnectionId, out int outChannelId, buffer, buffer.Length, out int receivedSize, out byte error);
				if (eventType == NetworkEventType.Nothing) break;

				Log.Verbose(LogTag, $"Received network event: {eventType}, from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}, Channel: {outChannelId}.");

				switch (eventType)
				{
					case NetworkEventType.ConnectEvent:
						HandleConnectEvent(receivedHostId, receivedConnectionId);
						break;
					case NetworkEventType.DisconnectEvent:
						Log.Verbose(LogTag, $"Disconnected from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}.");
						HandleDisconnectEvent(receivedHostId, receivedConnectionId);
						break;
					case NetworkEventType.DataEvent:
						Log.Verbose(LogTag, $"Received data from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}. \nRaw data: {buffer}.");
						HandleDataEvent(receivedHostId, receivedConnectionId, buffer);
						break;
					case NetworkEventType.BroadcastEvent:
						HandleBroadcastEvent(receivedHostId, receivedConnectionId, buffer);
						break;
				}
			} while (eventType != NetworkEventType.Nothing);
		}

		private void HandleConnectEvent(int receivedHostId, int receivedConnectionId)
		{
			NetHost host = GetHost(receivedHostId);
			if (host != null)
			{
				NetConnection existingConnection = host.GetConnection(receivedConnectionId);
				if (existingConnection != null)
				{
					existingConnection.ConfirmConnection();
				}
				else
				{
					NetConnection connection = new NetConnection(receivedConnectionId, host);
					host.AddConnection(connection);
					connection.ConfirmConnection();
				}
			}
			else
			{
				Log.WTF(LogTag, $"Received a connection event on a host that is not managed properly, HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}.");
			}
		}
		private void HandleDisconnectEvent(int receivedHostId, int receivedConnectionId)
		{
			NetHost host = GetHost(receivedHostId);
			NetConnection connection = host.GetConnection(receivedConnectionId);
			host.RemoveConnection(receivedConnectionId);

			host.HandleDisconnectEvent(connection);
			OnDisconnectEvent?.Raise(this, receivedConnectionId);
		}
		protected void HandleDataEvent(int receivedHostId, int receivedConnectionId, byte[] buffer)
		{
			NetDataPackage receivedDataPackage = NetDataPackage.DeserializeFrom(receivedConnectionId, buffer);

			NetConnection connection = GetHost(receivedHostId).GetConnection(receivedConnectionId);
			NetReceivedData receivedData = new NetReceivedData(connection, receivedDataPackage);

			NetDataEventManager.Instance.HandleDataEvent(receivedData);

			OnDataReceivedEvent?.Raise(this, receivedData);
		}
		protected void HandleBroadcastEvent(int receivedHostId, int receivedConnectionId, byte[] buffer)
		{
			byte[] broadcastConnectionMessageBuffer = new byte[2048];
			NetworkTransport.GetBroadcastConnectionMessage(receivedHostId, broadcastConnectionMessageBuffer, broadcastConnectionMessageBuffer.Length, out int receivedSize, out byte error);
			NetworkTransport.GetBroadcastConnectionInfo(receivedHostId, out string senderAddress, out int senderPort, out byte broadcastError);
			if (broadcastError == (int)NetworkError.Ok && error == (int)NetworkError.Ok)
			{

				NetDataPackage networkingDataPackage = NetDataPackage.DeserializeFrom(receivedConnectionId, broadcastConnectionMessageBuffer);

				NetHost host = GetHost(receivedHostId);
				int broadcastMessagePort = networkingDataPackage.GetDataAs<int>();
				Log.Verbose(LogTag, $"Received broadcast event from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}. Sender address: {senderAddress}, Sender port: {senderPort}. Broadcast message port: {broadcastMessagePort}.");
				ReceivedBroadcastData receivedBroadcastData = new ReceivedBroadcastData(host, senderAddress, senderPort, broadcastMessagePort);

				host.HandleBroadcastEvent(receivedBroadcastData);
				OnBroadcastEvent?.Raise(this, receivedBroadcastData);
			}
			else
			{
				Log.Warning(LogTag, $"Failed to read broadcast event data, GetBroadcastConnectionMessage error: {error}, GetBroadcastConnectionInfo error: {broadcastError}, from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}.");
			}
		}
		#endregion

		#region Host Management
		/// <returns>HostId</returns>
		public NetHost AddHost(int port = -1)
		{
			Initialize();
			var topology = new HostTopology(defaultConnectionConfig, options.maxConnections);

			int hostId = -1;
			if (port >= 0)
			{
				hostId = NetworkTransport.AddHost(topology, port);
			}
			else
			{
				hostId = NetworkTransport.AddHost(topology);
			}

			Log.Verbose(LogTag, $"Added host with id: {hostId}", this);

			NetHost netHost = NetHost.New(hostId, NetworkTransport.GetHostPort(hostId));
			activeHosts.Add(netHost);
			return netHost;
		}
		public bool RemoveHost(NetHost networkingHost)
		{
			if (IsInitialized == false) return false;
			if (networkingHost == null)
			{
				Log.Warning(LogTag, $"{nameof(RemoveHost)}: Host is null, aborting.");
				return false;
			}

			bool result = NetworkTransport.RemoveHost(networkingHost.Id);
			if (result)
			{
				Log.Verbose(LogTag, $"Removed host, HostId: {networkingHost.Id}.");
				networkingHost.Deactivate();
				activeHosts.Remove(networkingHost);
				Destroy(networkingHost);
			}
			else
			{
				Log.Warning(LogTag, $"Failed to remove host, HostId: {networkingHost.Id}");
			}
			return result;
		}

		public NetHost GetHost(int receivedHostId)
		{
			return activeHosts.Find(h => h.Id == receivedHostId);
		}
		#endregion

		#region Broadcast Discovery
		[ContextMenu("Start Broadcast Discovery")]
		public void StartBroadcastDiscovery()
		{
			StartBroadcastDiscovery(options.broadcastPort);
		}
		public void StartBroadcastDiscovery(int portNumberToBroadcast)
		{
			Initialize();
			if (IsBroadcasting) return;

			broadcastHost = AddHost();

			byte[] buffer = NetDataPackage.CreateFrom(portNumberToBroadcast).SerializeToByteArray();
			NetworkTransport.StartBroadcastDiscovery(broadcastHost.Id, options.broadcastPort, options.broadcastKey, options.broadcastVersion, options.broadcastSubversion, buffer, buffer.Length, 1000, out byte error);

			NetworkError networkError = (NetworkError)error;
			if (networkError == NetworkError.Ok)
			{
				Log.Info(LogTag, $"Started broadcasting on: HostId: {broadcastHost.Id}, Port: {options.broadcastPort}, Key: {options.broadcastKey}.");
			}
			else
			{
				Log.Error(LogTag, $"Failed to start broadcast discovery with error: {networkError}.");
			}
		}
		[ContextMenu("Stop Broadcast Discovery")]
		public void StopBroadcastDiscovery()
		{
			if (IsInitialized == false) return;
			if (IsBroadcasting)
			{
				NetworkTransport.StopBroadcastDiscovery();
				RemoveHost(broadcastHost);
				Log.Info(LogTag, $"Stopped broadcasting on: HostId: {broadcastHost.Id}, Port: {options.broadcastPort}");
			}
			if (broadcastHost != null)
			{
				broadcastHost = NetHost.Null;
			}
		}

		[ContextMenu("Start Scanning For Broadcast")]
		public void StartScanningForBroadcast()
		{
			Initialize();
			if (IsScanningForBroadcast) return;

			scanningHost = AddHost(options.broadcastPort);
			NetworkTransport.SetBroadcastCredentials(scanningHost.Id, options.broadcastKey, 1, 1, out byte error);

			NetworkError networkError = (NetworkError)error;
			if (networkError == NetworkError.Ok)
			{
				Log.Info(LogTag, $"Started scanning for broadcast on: HostId: {scanningHost.Id}, Key: {options.broadcastKey}");
			}
			else
			{
				Log.Error(LogTag, $"Failed to start scanning for broadcast with error: {networkError}.");
			}
		}
		[ContextMenu("Stop Scanning For Broadcast")]
		public void StopScanningForBroadcast()
		{
			if (IsInitialized == false) return;
			if (IsScanningForBroadcast)
			{
				Log.Info(LogTag, $"Stopping scanning for broadcast on: HostId: {scanningHost.Id}, Key: {options.broadcastKey}");
				RemoveHost(scanningHost);
			}
			scanningHost = NetHost.Null;
		}
		#endregion

		#region Managing connections
		public async Task<NetConnection> ConnectWithConfirmation(int hostId, string serverIP, int port, int timeoutMs = 1000)
		{
			NetConnection connection = AddConnection(hostId, serverIP, port);

			Task<bool> connectionConfirmationTask = connection.WaitForConnectionConfirmation(timeoutMs);
			bool result = false;
			try
			{
				result = await connectionConfirmationTask;
			}
			catch (TaskCanceledException) { }

			if (result)
			{
				return connection;
			}
			else
			{
				connection.Disconnect();
				return null;
			}
		}

		public NetConnection AddConnection(int hostId, string serverIP, int port)
		{
			var connectionId = NetworkTransport.Connect(hostId, serverIP, port, 0, out byte error);
			if ((NetworkError)error == NetworkError.Ok)
			{
				NetHost netHost = activeHosts.Find(h => h.Id == hostId);
				NetConnection connection = new NetConnection(connectionId, netHost);
				netHost.AddConnection(connection);

				Log.Verbose(LogTag, $"Added new connection, HostId: {hostId}, ConnectionId: {connectionId}.", this);

				return connection;
			}
			else
			{
				throw new Exception($"Failed to connect to server on hostId: {hostId}, serverIP: '{serverIP}', Error: {(NetworkError)error}");
			}
		}
		public NetworkError Disconnect(int hostId, int connectionId)
		{
			if (IsInitialized == false) return NetworkError.WrongOperation;
			NetworkTransport.Disconnect(hostId, connectionId, out byte error);
			Log.Verbose(LogTag, $"Disconnected from HostId: {hostId}, ConnectionId: {connectionId}.", this);
			return (NetworkError)error;
		}
		#endregion

		#region Cleanup
		public void OnDestroy() => Dispose();
		[ContextMenu("Dispose")]
		public void Dispose()
		{
			if (IsInitialized == false) return;

			Log.Info(LogTag, $"Disposing networking core...");
			StopBroadcastDiscovery();
			StopScanningForBroadcast();

			foreach (var host in new List<NetHost>(activeHosts))
			{
				RemoveHost(host);
			}

			IsInitialized = false;
			NetworkTransport.Shutdown();
			Log.Info(LogTag, $"Disposed networking core.");
		}
		#endregion

		#region Helpers
		private bool InitCheck()
		{
			if (IsInitialized == false)
			{
				Debug.LogError($"{nameof(NetCore)}: Error: The networking was not initialized, remember to call {nameof(Initialize)} before executing any other actions.");
				return false;
			}

			return true;
		}
		#endregion
	}
}
#pragma warning restore CS0618 // Type or member is obsolete

