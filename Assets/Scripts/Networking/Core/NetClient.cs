using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Networking
{
	public class NetClient : DontDestroySingleton<NetClient>
	{
		private static string LogTag = nameof(NetClient);

		#region Inner classes
		public enum ClientState
		{
			Uninitialized,
			NotConnected,
			LookingForServer,
			Connected
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
		public NetHost host;


		protected NetDataEventManager dataEventManager = new NetDataEventManager();


		#region Public properties
		public ClientState State { get => state; set => state = value; }
		public bool IsConnected { get => host.Connections.Count > 0; }
		public NetDataEventManager DataEventManager { get => dataEventManager; }
		#endregion


		#region Initialization
		protected void Awake()
		{
			host = NetCore.Instance.AddHost();
			host.OnDataEvent.RegisterListenerOnce(dataEventManager.HandleDataGameEvent);
		}

		private void OnEnable()
		{
			// Register event listeners
			NetCore.Instance.OnConnectEvent.RegisterListenerOnce(HandleConnect);
			NetCore.Instance.OnDisconnectEvent.RegisterListenerOnce(HandleDisconnect);
			NetCore.Instance.OnDataReceivedEvent.RegisterListenerOnce(HandleDataReceived);
			NetCore.Instance.OnBroadcastEvent.RegisterListenerOnce(HandleBroadcastEvent);
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
				NetConnection result = await host.ConnectWithConfirmation(broadcastData.senderAddress, broadcastData.senderPort);
				if (result != null)
				{
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

			host.Disconnect();
			OnDisconnect?.Raise(this);
		}

		private async Task<ReceivedBroadcastData> FindServerBroadcast(CancellationToken cancellationToken)
		{
			broadcastEventReceivedTaskCompletionSource = new TaskCompletionSource<ReceivedBroadcastData>();
			cancellationToken.Register(() => { broadcastEventReceivedTaskCompletionSource.TrySetCanceled(); });
			NetCore.Instance.StartScanningForBroadcast();

			// Wait for server to be found
			ReceivedBroadcastData broadcastData = await broadcastEventReceivedTaskCompletionSource.Task;
			NetCore.Instance.StopScanningForBroadcast();
			return broadcastData;
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

			var dataPackage = NetworkingDataPackage.CreateFrom(serializableData);
			var error = host.Send(channel, dataPackage.SerializeToByteArray());

			if (error == UnityEngine.Networking.NetworkError.Ok)
			{
				OnDataSent?.Raise(this, serializableData);
			}

			return error == UnityEngine.Networking.NetworkError.Ok;
		}
		/// <summary>
		/// Registers a listener to the OnDataReceived event that only gets called if the received data is of type T.
		/// </summary>
		/// <typeparam name="T">Type of data to listen to.</typeparam>
		/// <param name="action">Action to call when data is received.</param>
		public void RegisterDataHandler<T>(Action<T> action)
		{
			throw new NotImplementedException();
		}
		#endregion


		#region Handling Events
		protected void HandleConnect()
		{
			OnConnect?.Raise(this);
		}
		protected void HandleDisconnect()
		{
			OnDisconnect?.Raise(this);
		}
		protected void HandleDataReceived(GameEventData gameEventData)
		{
			if (gameEventData.data is NetworkingReceivedData receivedData)
			{
				dataEventManager.HandleDataEvent(receivedData);
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
		#endregion
	}
}
