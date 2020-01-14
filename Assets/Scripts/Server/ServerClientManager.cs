using Networking;
using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleBlast.Server
{
	[CreateAssetMenu(menuName = "BattleBlast/Systems/ServerClientManager")]
	public class ServerClientManager : ScriptableSystem
	{
		[SerializeField] protected List<NetConnection> unauthenticatedClientConnections = new List<NetConnection>();
		[SerializeField] protected List<ConnectedClient> connectedClients = new List<ConnectedClient>();

		protected DataHandler dataHandler;

		#region Initialization
		protected override void OnInitialize()
		{
			base.OnInitialize();

			dataHandler = DataHandler.New(HandleAuthenticationRequest, new NetDataFilterType(typeof(Credentials)));

			NetServer.Instance.host.OnConnectEvent.RegisterListenerOnce(HandleConnectGameEvent);
			NetDataEventManager.Instance.RegisterHandler(dataHandler);
		}
		#endregion

		#region Handling events
		public void HandleConnectGameEvent(GameEventData gameEventData)
		{
			if (gameEventData.data is NetConnection connection)
			{
				AddUnauthenticatedClient(connection);
			}
		}
		public void HandleDisconnectGameEvent(GameEventData gameEventData)
		{
			if (gameEventData.data is NetConnection connection)
			{
				ClientDisconnected(connection);
			}
		}
		#endregion

		#region Managing clients
		public void AddUnauthenticatedClient(NetConnection connection)
		{
			unauthenticatedClientConnections.Add(connection);

			connection.OnDisconnectEvent.RegisterListenerOnce(() => RemoveUnauthenticatedClient(connection));
		}
		public void RemoveUnauthenticatedClient(NetConnection connection)
		{
			unauthenticatedClientConnections.Remove(connection);
		}
		public void ClientDisconnected(NetConnection connection)
		{
			if (unauthenticatedClientConnections.Contains(connection)) RemoveUnauthenticatedClient(connection);

			var connectedClient = connectedClients.Find(c => c.Connection.Equals(connection));
			if (connectedClient != null) RemoveConnectedClient(connectedClient);
		}

		protected void AddConnectedClient(ConnectedClient connectedClient)
		{
			connectedClients.Add(connectedClient);
		}
		public void RemoveConnectedClient(ConnectedClient client)
		{
			connectedClients.Remove(client);
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
					receivedData.SendResponse(new AuthenticationResult(true, connectedClient.AuthToken, connectedClient.PlayerData.id));
				}
				else
				{
					receivedData.SendResponse(new AuthenticationResult(false, null, null));
				}
			}
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
