﻿using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

#pragma warning disable CS0618 // Type or member is obsolete
namespace Networking
{
	/// <summary>
	/// Low-level core networking class, abstracting the actual network implementation.
	/// </summary>
	[CreateAssetMenu(menuName = "Systems/Networking/Networking Core")]
	public class NetworkingCore : ScriptableSystem
	{
		#region Options
		[Serializable]
		public class Options
		{
			public int port = 8865;
			[Space]
			public int broadcastPort = 8866;
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

		[Header("Events")]
		public GameEventHandler OnConnectEvent = new GameEventHandler();
		public GameEventHandler OnDisconnectEvent = new GameEventHandler();
		public GameEventHandler OnDataReceivedEvent = new GameEventHandler();
		public GameEventHandler OnBroadcastEvent = new GameEventHandler();

		#region Initialization
		protected override void OnInitialize()
		{
			base.OnInitialize();

			NetworkTransport.Init();

			hostId = AddHost(Port);
		}
		#endregion

		#region Sending Data
		public virtual NetworkError Send(int connectionId, int channel, byte[] data)
		{
			if (InitCheck() == false) return NetworkError.WrongOperation;

			NetworkTransport.Send(HostId, connectionId, channel, data, data.Length, out byte error);
			return (NetworkError)error;
		}
		#endregion

		#region Handling Incoming Events
		public override void Update()
		{
			if (IsInitialized == false) return;

			byte[] buffer = new byte[2048];
			NetworkEventType eventType;
			do
			{
				eventType = NetworkTransport.Receive(out int receivedHostId, out int receivedConnectionId, out int outChannelId, buffer, buffer.Length, out int receivedSize, out byte error);
				switch (eventType)
				{
					case NetworkEventType.ConnectEvent:
						HandleConnectEvent(receivedHostId, receivedConnectionId);
						break;
					case NetworkEventType.DisconnectEvent:
						HandleDisconnectEvent(receivedConnectionId);
						break;
					case NetworkEventType.DataEvent:
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
		}
		private void HandleDisconnectEvent(int receivedConnectionId)
		{
			throw new NotImplementedException();
		}
		protected void HandleDataEvent(int receivedConnectionId, byte[] buffer)
		{
			throw new NotImplementedException();
		}
		protected void HandleBroadcastEvent(int receivedHostId, int receivedConnectionId)
		{
			byte[] buffer = new byte[2048];
			NetworkTransport.GetBroadcastConnectionMessage(scanningHostId, buffer, buffer.Length, out int receivedSize, out byte error);
			NetworkTransport.GetBroadcastConnectionInfo(scanningHostId, out string senderAddress, out int senderPort, out byte broadcastError);
			if (broadcastError == (int)NetworkError.Ok && error == (int)NetworkError.Ok)
			{
				DebugNet.Log("Found server.");
				serverIp = senderAddress;
				serverData = new ServerData(0, serverIp.Substring(7), Serializator.GetObject<string>(buffer));
				StartConnectingToServer(senderAddress);
			}


			throw new NotImplementedException();
		}
		#endregion

		#region Host Management
		/// <returns>HostId</returns>
		public int AddHost(int port = -1)
		{
			var topology = new HostTopology(defaultConnectionConfig, options.maxConnections);

			if (port >= 0)
			{
				return NetworkTransport.AddHost(topology, port);
			}
			else
			{
				return NetworkTransport.AddHost(topology);
			}
		}
		public bool RemoveHost(int hostId)
		{
			return NetworkTransport.RemoveHost(hostId);
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
		public int Connect(string serverIP)
		{
			var connectionId = NetworkTransport.Connect(HostId, serverIP, Port, 0, out byte error);
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

