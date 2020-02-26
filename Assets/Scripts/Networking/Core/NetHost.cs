using Athanor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

		[Header("NetHost variables")]
		[SerializeField] protected string hostName = "Unnamed";
		[SerializeField] [Disabled] protected int id = -1;
		[SerializeField] [Disabled] protected int port = -1;
		[SerializeField] [Disabled] protected bool isActive = false;

		[Header("Raised events")]
		public GameEventHandler OnDataEvent = new GameEventHandler();
		public GameEventHandler OnConnectEvent = new GameEventHandler();
		public GameEventHandler OnDisconnectEvent = new GameEventHandler();
		public GameEventHandler OnBroadcastEvent = new GameEventHandler();

		[Header("Connections")]
		[SerializeField] [Disabled] protected List<NetConnection> connections = new List<NetConnection>();


		#region Properties
		public int Id => id;
		public int Port => port;
		public bool IsActive => isActive;
		public List<NetConnection> Connections { get => new List<NetConnection>(connections); }
		public static NetHost Null => NetHost.New(-1, -1);
		#endregion


		#region Constructors
		public static NetHost New(int id, int port, string hostName = null)
		{
			NetHost newNetworkingHost = ScriptableObject.CreateInstance<NetHost>();
			newNetworkingHost.id = id;
			newNetworkingHost.port = port;
			newNetworkingHost.isActive = id >= 0 && port > 0;

			if (hostName == null) hostName = $"Unnamed ({Guid.NewGuid().ToString().Substring(0, 6)})";
			newNetworkingHost.hostName = hostName;

			return newNetworkingHost;
		}
		#endregion


		#region Handling events
		public void HandleDataEvent(NetReceivedData receivedData)
		{
			receivedData.connection.HandleDataEvent(receivedData);
			OnDataEvent?.Raise(this, receivedData);
		}
		public void HandleConnectEvent(NetConnection connection)
		{
			connection.HandleConnectEvent();
			OnConnectEvent?.Raise(this, connection);
		}
		public void HandleDisconnectEvent(NetConnection connection)
		{
			connection.HandleDisconnectEvent();
			OnDisconnectEvent?.Raise(this, connection);

			Destroy(connection);
		}
		public void HandleBroadcastEvent(ReceivedBroadcastData receivedBroadcastData)
		{
			OnBroadcastEvent?.Raise(this, receivedBroadcastData);
		}
		#endregion


		#region Managing connections
		public async Task<NetConnection> ConnectWithConfirmation(string serverIP, int port, CancellationToken cancellationToken = new CancellationToken())
		{
			return await NetCore.Instance.ConnectWithConfirmation(id, serverIP, port, cancellationToken);
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
			return error;
		}
		#endregion


		#region Deactivation
		public void Deactivate()
		{
			isActive = false;
		}
		#endregion


		#region Overrides
		public override string ToString()
		{
			return $"NetHost({hostName}, id: {id}, port: {port})";
		}
		#endregion
	}
}
