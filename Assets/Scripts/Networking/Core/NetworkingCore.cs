using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
	/// <summary>
	/// Low-level core networking class, abstracting the actual network implementation.
	/// </summary>
	public class NetworkingCore
	{
		protected static int hostId = -1;
		public static int HostId { get => hostId; }

		private static bool isInitialized = false;
		public static bool IsInitialized { get => isInitialized; }

		public static ConnectionConfig defaultConnectionConfig;

		public static void Init(int maxConnections = 1, int port = 8265)
		{
			NetworkTransport.Init();

			// Configure connection
			defaultConnectionConfig = new ConnectionConfig();
			Channel.reliable = defaultConnectionConfig.AddChannel(QosType.ReliableSequenced);
			Channel.unreliable = defaultConnectionConfig.AddChannel(QosType.Unreliable);

			HostTopology hostTopology = new HostTopology(defaultConnectionConfig, maxConnections);
			hostId = NetworkTransport.AddHost(hostTopology);
		}

		public static void StartBroadcasting()
		{
			if (InitCheck() == false) return;

			throw new NotImplementedException();

		}
		public static void StopBroadcasting()
		{
			throw new NotImplementedException();
		}

		public static void StartScanningForBroadcast()
		{
			throw new NotImplementedException();
		}
		public static void StopScanningForBroadcast()
		{
			throw new NotImplementedException();
		}

		private static bool InitCheck()
		{
			if (IsInitialized == false)
			{
				Debug.LogError($"{nameof(NetworkingCore)}: Error: The networking was not initialized, remember to call {nameof(Init)} before executing any other actions.");
				return false;
			}

			return true;
		}
	}
}
