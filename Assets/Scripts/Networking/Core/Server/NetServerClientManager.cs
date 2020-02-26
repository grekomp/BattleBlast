using Athanor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Networking
{
	[Serializable]
	public class NetServerClientManager
	{
		[Header("Runtime Variables")]
		public List<ConnectedClient> connectedClients = new List<ConnectedClient>();


		#region Adding clients
		public ConnectedClient NewClientConnected(NetConnection connection)
		{
			ConnectedClient connectedClient = GetClient(connection);
			if (connectedClient != null)
			{
				Log.Warning(this, $"Trying to create a new connected client for an existing connection. Make sure you only call {nameof(NewClientConnected)} once.");
				return connectedClient;
			}

			connectedClient = ConnectedClient.New(connection);
			connectedClients.Add(connectedClient);

			return connectedClient;
		}
		#endregion


		#region Removing clients
		public ConnectedClient ClientDisconnected(NetConnection connection)
		{
			ConnectedClient connectedClient = GetClient(connection);

			if (connectedClient == null)
			{
				Log.Warning(this, $"Trying to disconnect a client that was not managed or was already disconnected.");
				return null;
			}

			connectedClients.Remove(connectedClient);
			return connectedClient;
		}
		#endregion


		#region Searching for clients
		public ConnectedClient GetClient(NetConnection connection)
		{
			return connectedClients.Find(c => c.Connection == connection);
		}
		#endregion
	}
}
