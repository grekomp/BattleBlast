using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Utils;

namespace Networking
{
	[Serializable]
	public class NetHost : ScriptableObject
	{
		private static string LogTag = nameof(NetHost);

		[SerializeField] [Disabled] protected int id = -1;
		[SerializeField] [Disabled] protected int port = -1;

		[SerializeField] [Disabled] protected bool isActive = false;

		[SerializeField] [Disabled] protected List<NetConnection> connections = new List<NetConnection>();

		#region Properties
		public int Id => id;
		public int Port => port;
		public bool IsActive => isActive;
		public List<NetConnection> Connections { get => new List<NetConnection>(connections); }
		public static NetHost Null => NetHost.New(-1, -1);
		#endregion

		#region Constructors
		public static NetHost New(int id, int port)
		{
			NetHost newNetworkingHost = ScriptableObject.CreateInstance<NetHost>();
			newNetworkingHost.id = id;
			newNetworkingHost.port = port;
			newNetworkingHost.isActive = id >= 0 && port > 0;

			return newNetworkingHost;
		}
		#endregion

		#region Sending data
		/// <summary>
		/// Sends data to the first available connection
		/// </summary>
		public virtual NetworkError Send(int channel, byte[] data)
		{
			if (IsActive == false) return NetworkError.WrongHost;

			NetConnection connection = connections.First();
			if (connection == null)
			{
				Log.Warning(LogTag, $"Cannot send data - no connections available.");
				return NetworkError.WrongOperation;
			}

			return connection.Send(channel, data);
		}
		#endregion

		#region Handling events
		public void HandleReceivedEvent(NetworkEventType eventType, int receivedConnectionId, int channelId, byte[] buffer, int bufferSize, int receivedSize, byte error)
		{
			switch (eventType)
			{
				case NetworkEventType.DataEvent:
					break;
				case NetworkEventType.ConnectEvent:

					break;
				case NetworkEventType.DisconnectEvent:

					break;
				case NetworkEventType.Nothing:
					break;
				case NetworkEventType.BroadcastEvent:
					break;
				default:
					break;
			}
		}
		#endregion

		#region Scanning for broadcast
		public void StartScanningForBroadcast()
		{
			throw new NotImplementedException();
		}
		public void StopScanningForBroadcast()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Managing connections
		public async Task<NetConnection> ConnectWithConfirmation(string serverIP, int port)
		{
			return await NetCore.Instance.ConnectWithConfirmation(id, serverIP, port);
		}
		public NetConnection Connect(string serverIP, int port)
		{
			return NetCore.Instance.AddConnection(id, serverIP, port);
		}
		public void AddConnection(NetConnection netConnection)
		{
			if (connections.Contains(netConnection))
			{
				Log.WTF(LogTag, "New connection id is already present in the connections list, this should never happen.");
			}
			else
			{
				connections.Add(netConnection);
			}
		}
		public void RemoveConnection(int receivedConnectionId)
		{
			NetConnection connection = connections.Find(c => c.Id == receivedConnectionId);
			if (connection != null) connections.Remove(connection);
		}

		public NetConnection GetConnection(int connectionId)
		{
			return connections.Find(c => c.Id == connectionId);
		}
		public void Disconnect()
		{
			foreach (var connection in new List<NetConnection>(connections))
			{
				Disconnect(connection);
			}
		}
		public NetworkError Disconnect(int connectionId)
		{
			NetConnection connection = connections.Find(c => c.Id == connectionId);
			if (connection == null)
			{
				Log.Warning(LogTag, $"Failed to close connection id: {connectionId}, a connection with this id does not exist, or isn't managed properly by the host.");
				return NetworkError.WrongConnection;
			}

			return Disconnect(connection);
		}
		public NetworkError Disconnect(NetConnection connection)
		{
			NetworkError error = NetCore.Instance.Disconnect(id, connection.Id);
			connections.Remove(connection);
			return error;
		}
		#endregion

		public void Deactivate()
		{
			isActive = false;
		}


	}
}
