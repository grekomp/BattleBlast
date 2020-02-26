using Athanor;
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
		public static readonly string LogTagEvents = "Networking Event";

		#region Options
		[Serializable]
		public class Options
		{
			public int broadcastPort = 52489;
			public int broadcastKey = 8671;
			public int broadcastVersion = 1;
			public int broadcastSubversion = 1;
			[Space]
			public int maxConnections = 10;
		}


		[Header("Networking Core Options")]
		[SerializeField]
		protected Options options = new Options();
		public ConnectionConfigReference defaultConnectionConfig = new ConnectionConfigReference(new ConnectionConfig());

		public int BroadcastPort { get => options.broadcastPort; }
		#endregion


		[Header("Runtime Variables")]
		[SerializeField] [Disabled] protected NetHost broadcastHost;
		[SerializeField] [Disabled] protected NetHost scanningHost;
		[Space]
		[SerializeField] protected List<NetHost> activeHosts = new List<NetHost>();
		protected TaskCompletionSource<ReceivedBroadcastData> broadcastScanningTaskCompletionSource;

		[Header("Raised events")]
		public GameEventHandler OnConnectEvent = new GameEventHandler();
		public GameEventHandler OnDisconnectEvent = new GameEventHandler();
		public GameEventHandler OnDataReceivedEvent = new GameEventHandler();
		public GameEventHandler OnBroadcastEvent = new GameEventHandler();


		#region Properties
		public bool IsBroadcasting => NetworkTransport.IsBroadcastDiscoveryRunning();
		public bool IsScanningForBroadcast => scanningHost.Id >= 0;
		public bool IsInitialized { get; private set; }
		#endregion


		#region Initialization
		public void Awake() => Initialize();
		[ContextMenu("Initialize")]
		public void Initialize()
		{
			if (IsInitialized) return;

			Log.Verbose(LogTag, $"Initializing networking core: {this}.", this);
			if (NetworkTransport.IsStarted == false)
			{
				NetworkTransport.Init();
				Log.Verbose(LogTag, "Initialized NetworkTransport.", this);
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

			byte[] buffer = new byte[8000];
			NetworkEventType eventType;
			do
			{
				eventType = NetworkTransport.Receive(out int receivedHostId, out int receivedConnectionId, out int outChannelId, buffer, buffer.Length, out int receivedSize, out byte error);
				if (eventType == NetworkEventType.Nothing) break;

				Log.Verbose(LogTagEvents, $"Received network event: {eventType}, from: Host: {GetHost(receivedHostId)}, ConnectionId: {receivedConnectionId}, Channel: {outChannelId}.");

				switch (eventType)
				{
					case NetworkEventType.ConnectEvent:
						HandleConnectEvent(receivedHostId, receivedConnectionId);
						break;
					case NetworkEventType.DisconnectEvent:
						Log.Verbose(LogTagEvents, $"Disconnected from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}.");
						HandleDisconnectEvent(receivedHostId, receivedConnectionId);
						break;
					case NetworkEventType.DataEvent:
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
					Log.Info(LogTagEvents, $"Connection confirmation received - Host: {host}, Connection: {existingConnection}.");
					existingConnection.ConfirmConnection();
				}
				else
				{
					NetConnection connection = NetConnection.New(receivedConnectionId, host);
					Log.Info(LogTagEvents, $"New incoming connection - Host: {host}, Connection: {connection}.");
					host.AddConnection(connection);
					host.HandleConnectEvent(connection);
					connection.ConfirmConnection();
					OnConnectEvent?.Raise(this, connection);
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

			host.HandleDisconnectEvent(connection);
			host.RemoveConnection(receivedConnectionId);
			Utils.Utils.DestroyAnywhere(connection);

			OnDisconnectEvent?.Raise(this, receivedConnectionId);
		}
		protected void HandleDataEvent(int receivedHostId, int receivedConnectionId, byte[] buffer)
		{
			NetDataPackage receivedDataPackage = NetDataPackage.DeserializeFrom(buffer);

			Log.Verbose(LogTagEvents, $"Received data from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}. \nDataId: {receivedDataPackage.id}, Data: {receivedDataPackage.GetDataAs<object>()}.");

			NetHost host = GetHost(receivedHostId);
			NetConnection connection = host.GetConnection(receivedConnectionId);
			NetReceivedData receivedData = new NetReceivedData(connection, receivedDataPackage);

			NetDataEventManager.Instance.HandleDataEvent(receivedData);
			host.HandleDataEvent(receivedData);

			OnDataReceivedEvent?.Raise(this, receivedData);
		}
		protected void HandleBroadcastEvent(int receivedHostId, int receivedConnectionId, byte[] buffer)
		{
			byte[] broadcastConnectionMessageBuffer = new byte[2048];
			NetworkTransport.GetBroadcastConnectionMessage(receivedHostId, broadcastConnectionMessageBuffer, broadcastConnectionMessageBuffer.Length, out int receivedSize, out byte error);
			NetworkTransport.GetBroadcastConnectionInfo(receivedHostId, out string senderAddress, out int senderPort, out byte broadcastError);
			if (broadcastError == (int)NetworkError.Ok && error == (int)NetworkError.Ok)
			{

				NetDataPackage networkingDataPackage = NetDataPackage.DeserializeFrom(broadcastConnectionMessageBuffer);

				NetHost host = GetHost(receivedHostId);
				int broadcastMessagePort = networkingDataPackage.GetDataAs<int>();
				Log.Verbose(LogTagEvents, $"Received broadcast event from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}. Sender address: {senderAddress}, Sender port: {senderPort}. Broadcast message port: {broadcastMessagePort}.");
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
		public NetHost AddHost(int port = -1, string hostName = null)
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


			NetHost netHost = NetHost.New(hostId, NetworkTransport.GetHostPort(hostId), hostName);
			Log.Verbose(LogTag, $"Added Host: {netHost}.", this);
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
				Log.Verbose(LogTag, $"Removed Host: {networkingHost}.");
				networkingHost.Deactivate();
				activeHosts.Remove(networkingHost);
				ExtensionMethods.DestroyAnywhere(networkingHost);
			}
			else
			{
				Log.Warning(LogTag, $"Failed to remove Host: {networkingHost}");
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

			broadcastHost = AddHost(hostName: "Broadcast Host");

			byte[] buffer = NetDataPackage.CreateFrom(portNumberToBroadcast).SerializeToByteArray();
			NetworkTransport.StartBroadcastDiscovery(broadcastHost.Id, options.broadcastPort, options.broadcastKey, options.broadcastVersion, options.broadcastSubversion, buffer, buffer.Length, 1000, out byte error);

			NetworkError networkError = (NetworkError)error;
			if (networkError == NetworkError.Ok)
			{
				Log.Info(LogTag, $"Started broadcasting on: Host: {broadcastHost}, Port: {options.broadcastPort}, Key: {options.broadcastKey}.");
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
				Log.Info(LogTag, $"Stopped broadcasting on: Host: {broadcastHost}, Port: {options.broadcastPort}");
			}
			if (broadcastHost != null)
			{
				broadcastHost = NetHost.Null;
			}
		}

		[ContextMenu("Start Scanning For Broadcast")]
		public IEnumerator StartScanningForBroadcast(ref ReceivedBroadcastData broadcastData, CancellationToken cancellationToken = new CancellationToken())
		{
			Initialize();
			if (IsScanningForBroadcast) return Task.Run(async () => await broadcastScanningTaskCompletionSource.Task, cancellationToken);

			scanningHost = AddHost(options.broadcastPort, "Broadcast scanning host");
			NetworkTransport.SetBroadcastCredentials(scanningHost.Id, options.broadcastKey, 1, 1, out byte error);

			NetworkError networkError = (NetworkError)error;
			if (networkError == NetworkError.Ok)
			{
				Log.Info(LogTag, $"Started scanning for broadcast on: Host: {scanningHost}, Key: {options.broadcastKey}");
			}
			else
			{
				Log.Error(LogTag, $"Failed to start scanning for broadcast with error: {networkError}.");
			}

			return Task.Run(async () => await broadcastScanningTaskCompletionSource.Task, cancellationToken);
		}
		[ContextMenu("Stop Scanning For Broadcast")]
		public void StopScanningForBroadcast()
		{
			if (IsInitialized == false) return;
			if (IsScanningForBroadcast)
			{
				Log.Info(LogTag, $"Stopping scanning for broadcast on: Host: {scanningHost}, Key: {options.broadcastKey}");
				RemoveHost(scanningHost);
			}
			scanningHost = NetHost.Null;
		}

		public IEnumerator WaitForBroadcastScanningSuccess(ref ReceivedBroadcastData broadcastData, CancellationToken cancellationToken)
		{
			await Task.Run(async () => await broadcastScanningTaskCompletionSource.Task, cancellationToken);

			return await broadcastScanningTaskCompletionSource.Task;
		}
		#endregion


		#region Managing connections
		public async Task<NetConnection> ConnectWithConfirmation(int hostId, string serverIP, int port, CancellationToken cancellationToken = new CancellationToken())
		{
			NetConnection connection = AddConnection(hostId, serverIP, port);

			Task<bool> connectionConfirmationTask = connection.WaitForConnectionConfirmation(cancellationToken);
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
				NetConnection connection = NetConnection.New(connectionId, netHost);
				netHost.AddConnection(connection);

				Log.Verbose(LogTag, $"New outgoing connection, Host: {netHost}, Connection: {connection}.", this);

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

