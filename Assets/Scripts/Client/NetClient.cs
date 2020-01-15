using Networking;
using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace BattleBlast
{
	public class NetClient : DontDestroySingleton<NetClient>
	{
		private static readonly string LogTag = nameof(NetClient);

		#region Inner classes
		public enum ClientState
		{
			Uninitialized,
			NotConnected,
			LookingForServer,
			Connected,
			Authenticated
		}

		[Serializable]
		public class NetClientSettings
		{
			public int connectToServerTimeout = 100;
		}
		#endregion


		[Header("Networking Client Options")]
		[SerializeField] protected NetClientSettings networkingClientSettings;

		[Header("Runtime Variables")]
		[SerializeField] [Disabled] protected ClientState state;
		[SerializeField] [Disabled] protected NetHost host;
		public NetConnection connection;

		[SerializeField] [Disabled] protected string authToken = "";
		[SerializeField] [Disabled] protected string playerId = "";


		#region Public properties
		public ClientState State => state;
		public bool IsConnected => connection != null;
		public string AuthToken => authToken;
		public string PlayerId => playerId;
		public NetHost Host => host;
		#endregion


		#region Initialization
		protected void Awake()
		{
			host = NetCore.Instance.AddHost();
		}

		private void OnEnable()
		{
			// Register event listeners
			host.OnConnectEvent.RegisterListenerOnce(HandleConnect);
			NetCore.Instance.OnBroadcastEvent.RegisterListenerOnce(HandleBroadcastEvent);

			TryConnectToServer();
		}
		private void OnDisable()
		{
			if (NetCore.InstanceExists == false) return;

			// Deregister event listeners
			NetCore.Instance.OnConnectEvent.DeregisterListener(HandleConnect);
			NetCore.Instance.OnDisconnectEvent.DeregisterListener(HandleDisconnect);
			NetCore.Instance.OnDataReceivedEvent.DeregisterListener(HandleDataReceived);
			NetCore.Instance.OnBroadcastEvent.DeregisterListener(HandleBroadcastEvent);
		}
		#endregion


		#region Managing Connection
		protected TaskCompletionSource<ReceivedBroadcastData> broadcastEventReceivedTaskCompletionSource;

		[ContextMenu(nameof(TryConnectToServer))]
		/// <summary>
		/// Attempts to automatically connect to any available server.
		/// </summary>
		/// <returns></returns>
		public async void TryConnectToServer()
		{
			if (IsConnected) return;

			Log.Info(LogTag, "Trying to connect to server...", this);

			bool result = false;
			try
			{
				result = await ConnectToServer(TaskExtensions.GetTimeoutCancellationToken(networkingClientSettings.connectToServerTimeout));
			}
			catch (TaskCanceledException)
			{
				Log.Verbose(LogTag, "Connecting to server timed out.", this);
			}

			if (result)
			{
				Log.Info(LogTag, "Connected to server.", this);
			}
			else
			{
				state = ClientState.NotConnected;
				Log.Info(LogTag, "Failed to connect to server.", this);
			}
		}
		public async Task<bool> ConnectToServer(CancellationToken cancellationToken)
		{
			if (IsConnected) return true;

			// Start looking for server
			state = ClientState.LookingForServer;
			var task = FindServerBroadcast(cancellationToken);
			ReceivedBroadcastData broadcastData = await task;

			// Connect to the found server
			if (task.Status == TaskStatus.RanToCompletion)
			{
				connection = await host.ConnectWithConfirmation(broadcastData.senderAddress, broadcastData.broadcastMessagePort);
				if (connection != null)
				{
					connection.OnDisconnectEvent.RegisterListenerOnce(HandleDisconnect);
					connection.OnDataEvent.RegisterListenerOnce(HandleDataReceived);

					state = ClientState.Connected;
					return true;
				}
				else
				{
					state = ClientState.NotConnected;
					return false;
				}
			}
			else
			{
				state = ClientState.NotConnected;
				return false;
			}
		}

		[ContextMenu(nameof(DisconnectFromServer))]
		public void DisconnectFromServer()
		{
			if (IsConnected == false) return;

			Log.Info(LogTag, "Disconnecting from server...", this);
			connection.Disconnect();
			Log.Info(LogTag, "Disconnected.", this);
		}

		private async Task<ReceivedBroadcastData> FindServerBroadcast(CancellationToken cancellationToken)
		{
			broadcastEventReceivedTaskCompletionSource = new TaskCompletionSource<ReceivedBroadcastData>();
			cancellationToken.Register(() => { broadcastEventReceivedTaskCompletionSource.TrySetCanceled(); });
			NetCore.Instance.StartScanningForBroadcast();

			// Wait for server to be found
			ReceivedBroadcastData broadcastData = null;
			try
			{
				broadcastData = await broadcastEventReceivedTaskCompletionSource.Task;
			}
			finally
			{
				NetCore.Instance.StopScanningForBroadcast();
			}
			return broadcastData;
		}
		#endregion


		#region MyRegion
		public async Task<bool> TryAuthenticate(Credentials credentials)
		{
			if (IsConnected == false) return false;

			Log.Info(LogTag, $"Attempting to authenticate player. Username: {credentials.username}.");
			NetReceivedData response = await NetRequest.CreateAndSend(connection, credentials).WaitForResponse();

			if (response.data is AuthenticationResult authenticationResult)
			{
				if (authenticationResult.authenticationSuccessfull)
				{
					authToken = authenticationResult.authToken;
					playerId = authenticationResult.playerId;

					state = ClientState.Authenticated;

					Log.Info(LogTag, "Authentication successfull.");
					return true;
				}
				else
				{
					Log.Info(LogTag, "Authentication failed.");
				}
			}
			else
			{
				Log.Error(LogTag, "Authentication error: Unexpected response type.");
			}

			return false;
		}
		#endregion


		#region Handling Data
		/// <summary>
		/// Sends provided serializableData object to the server.
		/// </summary>
		/// <param name="serializableData">Data object to send. Must be a serializable class.</param>
		/// <returns>Whether the operation succeeded.</returns>
		public bool SendData(object serializableData, int channel = Channel.ReliableSequenced)
		{
			if (IsConnected == false) return false;

			var result = connection.Send(serializableData, channel);
			if (result == UnityEngine.Networking.NetworkError.Ok)
			{
				OnDataSent?.Raise(this, serializableData);
			}

			return result == UnityEngine.Networking.NetworkError.Ok;
		}
		/// <summary>
		/// Registers a listener to the OnDataReceived event that only gets called if the received data is of type T.
		/// </summary>
		/// <typeparam name="T">Type of data to listen to.</typeparam>
		/// <param name="action">Action to call when data is received.</param>
		#endregion


		#region Handling Events
		protected void HandleConnect()
		{
			OnConnect?.Raise(this);
		}
		protected void HandleDisconnect()
		{
			state = ClientState.NotConnected;
			authToken = null;
			playerId = null;
			connection = null;

			OnDisconnect?.Raise(this);
		}
		protected void HandleDataReceived(GameEventData gameEventData)
		{
			if (gameEventData.data is NetReceivedData receivedData)
			{
				OnDataReceived?.Raise(this, receivedData);
			}
		}
		protected void HandleDataSent()
		{
			OnDataSent?.Raise(this);
		}
		protected void HandleBroadcastEvent(GameEventData gameEventData)
		{
			if (state != ClientState.LookingForServer) return;

			if (gameEventData.data is ReceivedBroadcastData receivedBroadcastData)
			{
				if (broadcastEventReceivedTaskCompletionSource != null)
				{
					broadcastEventReceivedTaskCompletionSource.TrySetResult(receivedBroadcastData);
				}
			}
		}

		[Header("Events")]
		public GameEventHandler OnConnect;
		public GameEventHandler OnDisconnect;
		public GameEventHandler OnDataReceived;
		public GameEventHandler OnDataSent;
		#endregion


		#region Debug
		[ContextMenu("SendTestData")]
		public void SendTestData()
		{
			SendData("Test data!");
		}
		[ContextMenu(nameof(SendTestRequest))]
		public async void SendTestRequest()
		{
			var request = NetRequest.CreateAndSend(connection, "Test request data");
			var result = await request.WaitForResponse();
			Log.D(result.data);
		}
		#endregion
	}
}
