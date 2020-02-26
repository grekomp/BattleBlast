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
		public static ServerClientManager Instance => BBServer.Instance.Systems.ClientManager;


		[SerializeField] protected List<NetConnection> unauthenticatedClientConnections = new List<NetConnection>();
		[SerializeField] private List<BBConnectedClient> connectedClients = new List<BBConnectedClient>();

		protected DataHandler dataHandler;

		public List<BBConnectedClient> ConnectedClients => new List<BBConnectedClient>(connectedClients);


		#region Initialization
		protected override void OnInitialize()
		{
			base.OnInitialize();

			dataHandler = DataHandler.New(HandleAuthenticationRequest, new NetDataFilterType(typeof(Credentials)));

			BBServer.Instance.host.OnConnectEvent.RegisterListenerOnce(HandleConnectGameEvent);
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

			var connectedClient = connectedClients.Find(c => c.NetworkingClient.Connection.Equals(connection));
			if (connectedClient != null) RemoveConnectedClient(connectedClient);
		}

		public void DisconnectAllClients()
		{
			foreach (var connection in new List<NetConnection>(unauthenticatedClientConnections))
			{
				connection?.Disconnect();
			}

			foreach (var connectedClient in new List<BBConnectedClient>(connectedClients))
			{
				connectedClient?.NetworkingClient?.Connection?.Disconnect();
			}

			unauthenticatedClientConnections.Clear();
			connectedClients.Clear();
		}
		protected void AddConnectedClient(BBConnectedClient connectedClient)
		{
			connectedClients.Add(connectedClient);
		}
		public void RemoveConnectedClient(BBConnectedClient client)
		{
			connectedClients.Remove(client);
		}


		public BBConnectedClient GetClientForPlayer(PlayerData playerData)
		{
			return connectedClients.Find(c => c.PlayerData.Equals(playerData));
		}
		#endregion


		#region Authentication
		public void HandleAuthenticationRequest(NetReceivedData receivedData)
		{
			if (receivedData.data is Credentials credentials)
			{
				if (TryAuthenticateClient(receivedData.connection, credentials, out BBConnectedClient connectedClient))
				{
					RemoveUnauthenticatedClient(connectedClient.NetworkingClient.Connection);
					AddConnectedClient(connectedClient);
					receivedData.SendResponse(new AuthenticationResult(true, connectedClient.AuthToken, connectedClient.PlayerData.id));
				}
				else
				{
					receivedData.SendResponse(new AuthenticationResult(false, null, null));
				}
			}
		}

		public bool TryAuthenticateClient(NetConnection connection, Credentials credentials, out BBConnectedClient connectedClient)
		{
			bool authenticationResult = Authenticator.TryAuthenticatePlayer(credentials, out PlayerData playerData, out string authToken);
			if (authenticationResult)
			{
				connectedClient = new BBConnectedClient() { PlayerData = playerData, AuthToken = authToken };
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
