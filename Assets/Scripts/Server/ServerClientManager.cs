﻿using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleBlast.Server
{
	public class ServerClientManager
	{
		[SerializeField] protected List<NetConnection> unauthenticatedClientConnections = new List<NetConnection>();

		#region Initialization
		public void Initialize()
		{

		}
		#endregion

		#region Unauthenticated clients
		public void AddUnauthenticatedClient(NetConnection connection)
		{
			unauthenticatedClientConnections.Add(connection);

			connection.OnDisconnectEvent.RegisterListenerOnce(() => RemoveUnauthenticatedClient(connection));
		}
		public void RemoveUnauthenticatedClient(NetConnection connection)
		{
			unauthenticatedClientConnections.Remove(connection);
		}
		#endregion

		#region Authentication
		public void HandleAuthenticationRequest(NetReceivedData receivedData)
		{
			if (receivedData.data is Credentials credentials)
			{
				if (TryAuthenticateClient(receivedData.connection, credentials, out ConnectedClient connectedClient))
				{
					RemoveUnauthenticatedClient(connectedClient.Connection);
					AddConnectedClient(connectedClient);
					// TODO: Send authentication confirmed
				}
				else
				{
					// TODO: Send authentication failed
				}
			}
		}

		private void AddConnectedClient(ConnectedClient connectedClient)
		{
			throw new NotImplementedException();
		}

		public bool TryAuthenticateClient(NetConnection connection, Credentials credentials, out ConnectedClient connectedClient)
		{
			bool authenticationResult = Authenticator.TryAuthenticatePlayer(credentials, out PlayerData playerData, out string authToken);
			if (authenticationResult)
			{
				connectedClient = ConnectedClient.New(connection, playerData, authToken);
				return true;
			}
			else
			{
				connectedClient = null;
				return false;
			}
		}
		#endregion


	}
}
