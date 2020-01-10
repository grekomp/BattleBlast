using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	[CreateAssetMenu(menuName = "Systems/Networking/Networking Core")]
	public class NetworkingCore : ScriptableObject
	{
		public static readonly string LogTag = "Networking";

		#region Options
		[Serializable]
		public class Options
		{
			public int port = 8892;
			[Space]
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

		public int Port { get => options.port; }
		public int BroadcastPort { get => options.broadcastPort; }
		#endregion

		[Header("Runtime Variables")]
		[SerializeField]
		[Disabled]
		protected int hostId = -1;
		public int HostId { get => hostId; }
		[SerializeField]
		[Disabled]
		protected int broadcastHostId = -1;
		public int BroadcastHostId { get => broadcastHostId; }
		[SerializeField]
		[Disabled]
		protected int scanningHostId = -1;
		public int ScanningHostId { get => scanningHostId; }

		private bool IsBroadcasting => NetworkTransport.IsBroadcastDiscoveryRunning();
		private bool IsScanningForBroadcast => ScanningHostId >= 0;

		public bool IsInitialized { get; private set; }

		[Header("Events")]
		public GameEventHandler OnConnectEvent = new GameEventHandler();
		public GameEventHandler OnDisconnectEvent = new GameEventHandler();
		public GameEventHandler OnDataReceivedEvent = new GameEventHandler();
		public GameEventHandler OnBroadcastEvent = new GameEventHandler();

		#region Initialization
		public void Initialize()
		{
			if (NetworkTransport.IsStarted == false)
			{
				NetworkTransport.Init();
				Log.Info(LogTag, "Initialized NetworkTransport.", this);
			}

			AddDefaultHost();
			IsInitialized = true;
		}

		private void AddDefaultHost()
		{
			hostId = AddHost(Port);
		}
		#endregion

		#region Sending Data
		public virtual NetworkError Send(int connectionId, int channel, byte[] data)
		{
			if (InitCheck() == false) return NetworkError.WrongOperation;

			NetworkTransport.Send(HostId, connectionId, channel, data, data.Length, out byte error);

			NetworkError networkError = (NetworkError)error;
			if (networkError == NetworkError.Ok)
			{
				Log.Verbose(LogTag, $"Sent data via: HostId: {HostId}, ConnectionId: {connectionId}, Channel: {channel}. \nData: {data}.", this);
			}
			else
			{
				Log.Warning(LogTag, $"Failed to send data with error: {networkError} via HostId: {HostId}, ConnectionId: {connectionId}, Channel: {channel}. \nData: {data}.", this);
			}

			return networkError;
		}
		#endregion

		#region Handling Incoming Events
		public void Update()
		{
			if (IsInitialized == false) return;

			byte[] buffer = new byte[2048];
			NetworkEventType eventType;
			do
			{
				eventType = NetworkTransport.Receive(out int receivedHostId, out int receivedConnectionId, out int outChannelId, buffer, buffer.Length, out int receivedSize, out byte error);
				Log.Verbose(LogTag, $"Received network event: {eventType}, from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}, Channel: {outChannelId}.");

				switch (eventType)
				{
					case NetworkEventType.ConnectEvent:
						HandleConnectEvent(receivedHostId, receivedConnectionId);
						break;
					case NetworkEventType.DisconnectEvent:
						Log.Verbose(LogTag, $"Disconnected from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}.");
						HandleDisconnectEvent(receivedConnectionId);
						break;
					case NetworkEventType.DataEvent:
						Log.Verbose(LogTag, $"Received data from: HostId: {receivedHostId}, ConnectionId: {receivedConnectionId}. \nRaw data: {buffer}.");
						HandleDataEvent(receivedConnectionId, buffer);
						break;
					case NetworkEventType.BroadcastEvent:
						HandleBroadcastEvent(receivedHostId, receivedConnectionId);
						break;
				}
			} while (eventType != NetworkEventType.Nothing);
		}

		private void HandleConnectEvent(int receivedHostId, int receivedConnectionId)
		{
			NetworkTransport.GetConnectionInfo(receivedHostId, receivedConnectionId, out string outIp, out int outPort, out NetworkID outNetwork, out NodeID outDstNode, out byte error);

			// TODO: Finish implementation
		}
		private void HandleDisconnectEvent(int receivedConnectionId)
		{
			OnDisconnectEvent?.Raise(this, receivedConnectionId);
		}
		protected void HandleDataEvent(int receivedConnectionId, byte[] buffer)
		{
			NetworkingDataPackage receivedDataPackage = NetworkingDataPackage.DeserializeFrom(receivedConnectionId, buffer);
			OnDataReceivedEvent?.Raise(this, new NetworkingReceivedData(receivedConnectionId, receivedDataPackage));
		}
		protected void HandleBroadcastEvent(int receivedHostId, int receivedConnectionId)
		{
			byte[] buffer = new byte[2048];
			NetworkTransport.GetBroadcastConnectionMessage(scanningHostId, buffer, buffer.Length, out int receivedSize, out byte error);
			NetworkTransport.GetBroadcastConnectionInfo(scanningHostId, out string senderAddress, out int senderPort, out byte broadcastError);
			if (broadcastError == (int)NetworkError.Ok && error == (int)NetworkError.Ok)
			{
				Log.Verbose(LogTag, $"Received broadcast event from: HostId: {scanningHostId}, ConnectionId: {receivedConnectionId}. Sender address: {senderAddress}, Sender port: {senderPort}. \nRaw data: {buffer}.");

				ReceivedBroadcastData receivedBroadcastData = new ReceivedBroadcastData(senderAddress, senderPort);
				OnBroadcastEvent?.Raise(this, receivedBroadcastData);
			}
			else
			{
				Log.Warning(LogTag, $"Failed to read broadcast event data, GetBroadcastConnectionMessage error: {error}, GetBroadcastConnectionInfo error: {broadcastError}, from: HostId: {scanningHostId}, ConnectionId: {receivedConnectionId}.");
			}
		}
		#endregion

		#region Host Management
		/// <returns>HostId</returns>
		public int AddHost(int port = -1)
		{
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

			Log.Info(LogTag, $"Added host with id: {hostId}", this);
			return hostId;
		}
		public bool RemoveHost(int hostId)
		{
			bool result = NetworkTransport.RemoveHost(hostId);
			if (result)
			{
				Log.Info(LogTag, $"Removed host, HostId: {hostId}.");
			}
			else
			{
				Log.Warning(LogTag, $"Failed to remove host, HostId: {hostId}");
			}
			return result;
		}
		#endregion

		#region Broadcast Discovery
		public void StartBroadcastDiscovery()
		{
			if (InitCheck() == false) return;
			if (IsBroadcasting) return;

			broadcastHostId = AddHost();

			byte[] buffer = Utils.ObjectSerializationExtension.SerializeToByteArray(SystemInfo.deviceName);
			NetworkTransport.StartBroadcastDiscovery(broadcastHostId, options.broadcastPort, options.broadcastKey, options.broadcastVersion, options.broadcastSubversion, buffer, buffer.Length, 1000, out byte error);

			NetworkError networkError = (NetworkError)error;
			if (networkError == NetworkError.Ok)
			{
				Log.Info(LogTag, $"Started broadcasting on: HostId: {broadcastHostId}, Port: {options.broadcastPort}");
			}
		}
		public void StopBroadcastDiscovery()
		{
			NetworkTransport.StopBroadcastDiscovery();
			RemoveHost(broadcastHostId);
		}

		public void StartScanningForBroadcast()
		{
			if (IsScanningForBroadcast) return;

			scanningHostId = AddHost(options.broadcastPort);
			NetworkTransport.SetBroadcastCredentials(scanningHostId, options.broadcastKey, 1, 1, out byte error);
		}
		public void StopScanningForBroadcast()
		{
			if (IsScanningForBroadcast)
			{
				RemoveHost(scanningHostId);
				scanningHostId = -1;
			}
		}
		#endregion

		#region Managing connections
		public int Connect(string serverIP, int port = -1)
		{
			if (port < 0)
			{
				port = Port;
			}

			var connectionId = NetworkTransport.Connect(HostId, serverIP, port, 0, out byte error);
			if ((NetworkError)error == NetworkError.Ok)
			{
				return connectionId;
			}
			else
			{
				throw new Exception($"Failed to connect to server on hostId: {HostId}, serverIP: '{serverIP}', Error: {(NetworkError)error}");
			}
		}
		public NetworkError Disconnect(int connectionId)
		{
			NetworkTransport.Disconnect(HostId, connectionId, out byte error);
			return (NetworkError)error;
		}
		#endregion

		#region Cleanup
		public void Dispose()
		{
			if (IsInitialized == false) return;

			StopBroadcastDiscovery();
			StopScanningForBroadcast();
			RemoveHost(HostId);

			Log.Info(LogTag, $"Disposed networking core: {this}");
		}
		#endregion

		#region Helpers
		private bool InitCheck()
		{
			if (IsInitialized == false)
			{
				Debug.LogError($"{nameof(NetworkingCore)}: Error: The networking was not initialized, remember to call {nameof(Initialize)} before executing any other actions.");
				return false;
			}

			return true;
		}
		#endregion
	}
}
#pragma warning restore CS0618 // Type or member is obsolete

