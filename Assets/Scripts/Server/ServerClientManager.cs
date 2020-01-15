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
		public static ServerClientManager Instance => NetServer.Instance.Systems.ClientManager;


		[SerializeField] protected List<NetConnection> unauthenticatedClientConnections = new List<NetConnection>();
		[SerializeField] private List<ConnectedClient> connectedClients = new List<ConnectedClient>();

		protected DataHandler dataHandler;

		public List<ConnectedClient> ConnectedClients => new List<ConnectedClient>(connectedClients);


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

		public void DisconnectAllClients()
		{
			foreach (var connection in new List<NetConnection>(unauthenticatedClientConnections))
			{
				connection?.Disconnect();
			}

			foreach (var connectedClient in new List<ConnectedClient>(connectedClients))
			{
				connectedClient?.Connection?.Disconnect();
			}

			unauthenticatedClientConnections.Clear();
			connectedClients.Clear();
		}
		protected void AddConnectedClient(ConnectedClient connectedClient)
		{
			connectedClients.Add(connectedClient);
		}
		public void RemoveConnectedClient(ConnectedClient client)
		{
			connectedClients.Remove(client);
		}


		public ConnectedClient GetClientForPlayer(PlayerData playerData)
		{
			return connectedClients.Find(c => c.PlayerData.Equals(playerData));
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


		#region Cleanup
		public override void Dispose()
		{
			base.Dispose();

			DisconnectAllClients();
			NetDataEventManager.Instance.DeregisterHandler(dataHandler);
		}
		#endregion
	}
}
