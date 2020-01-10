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
	public class NetworkingClient : DontDestroySingleton<NetworkingClient>
	{
		private static string LogTag = nameof(NetworkingClient);

		[Serializable]
		public class ServerInfo
		{
			public int connectionId = -1;
			public string address;
			public int port;

			public ServerInfo(int connectionId, string address, int port)
			{
				this.connectionId = connectionId;
				this.address = address;
				this.port = port;
			}
			public ServerInfo(string address, int port)
			{
				connectionId = -1;
				this.address = address;
				this.port = port;
			}
		}

		[Serializable]
		public class NetworkingClientSettings
		{
			public int connectToServerTimeout = 100;
		}

		[Header("Networking Client Options")]
		[SerializeField] protected NetworkingCore networkingCore;
		[SerializeField] protected NetworkingClientSettings networkingClientSettings;

		[Header("Runtime Variables")]
		[SerializeField]
		[Disabled]
		protected ServerInfo connectedServerInfo = null;

		public bool IsConnected { get => connectedServerInfo != null && connectedServerInfo.connectionId >= 0; }


		#region Initialization
		protected void Awake()
		{

			if (networkingCore == null) networkingCore = ScriptableObject.CreateInstance<NetworkingCore>();
			networkingCore.Initialize();

			networkingCore.OnConnectEvent.RegisterListenerOnce(HandleConnect);
			networkingCore.OnDisconnectEvent.RegisterListenerOnce(HandleDisconnect);
		}
		#endregion

		#region Managing Connection
		protected TaskCompletionSource<ReceivedBroadcastData> broadcastEventReceivedTaskCompletionSource;

		[ContextMenu("TryConnectToServer")]
		/// <summary>
		/// Attempts to automatically connect to any available server.
		/// </summary>
		/// <returns></returns>
		public async void TryConnectToServer()
		{
			Log.Info(LogTag, "Trying to connect to server...", this);

			bool result = false;
			try
			{
				result = await ConnectToServer(TaskExtensions.GetTimeoutCancellationToken(networkingClientSettings.connectToServerTimeout));

			}
			catch (TaskCanceledException exception)
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
			// Start looking for server
			broadcastEventReceivedTaskCompletionSource = new TaskCompletionSource<ReceivedBroadcastData>();
			cancellationToken.Register(() => { broadcastEventReceivedTaskCompletionSource.TrySetCanceled(); });
			networkingCore.OnBroadcastEvent.RegisterListenerOnce(HandleBroadcastEvent);
			networkingCore.StartScanningForBroadcast();

			// Wait for server to be found
			ReceivedBroadcastData broadcastData = await broadcastEventReceivedTaskCompletionSource.Task;
			networkingCore.OnBroadcastEvent.DeregisterListener(HandleBroadcastEvent);
			networkingCore.StopScanningForBroadcast();

			// Connect to the found server
			if (broadcastEventReceivedTaskCompletionSource.Task.Status == TaskStatus.RanToCompletion)
			{
				ServerInfo foundServerInfo = new ServerInfo(broadcastData.senderAddress, broadcastData.senderPort);
				foundServerInfo.connectionId = networkingCore.Connect(foundServerInfo.address, foundServerInfo.port);
				return true;
			}
			else
			{
				return false;
			}
		}
		public void DisconnectFromServer()
		{
			if (IsConnected == false) return;
			networkingCore.Disconnect(connectedServerInfo.connectionId);
			connectedServerInfo.connectionId = -1;

			OnDisconnect?.Raise(this);
		}

		protected void HandleBroadcastEvent(GameEventData gameEventData)
		{
			if (gameEventData.data is ReceivedBroadcastData receivedBroadcastData)
			{
				if (broadcastEventReceivedTaskCompletionSource != null)
				{
					broadcastEventReceivedTaskCompletionSource.TrySetResult(receivedBroadcastData);
				}
			}
		}
		#endregion

		#region Handling Data
		/// <summary>
		/// Sends provided serializableData object to the server.
		/// </summary>
		/// <param name="serializableData">Data object to send. Must be a serializable class.</param>
		/// <returns>Whether the operation succeeded.</returns>
		public bool SendData(object serializableData)
		{
			if (IsConnected == false) return false;

			var dataPackage = NetworkingDataPackage.CreateFrom(serializableData);
			var error = networkingCore.Send(connectedServerInfo.connectionId, Channel.reliable, dataPackage.SerializeToByteArray());

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

		protected void HandleDataReceived(GameEventData gameEventData)
		{
			if (gameEventData.data is NetworkingReceivedData receivedData)
			{
				OnDataReceived?.Raise(this, receivedData);
			}
		}
		#endregion

		#region Handling Events
		public void HandleConnect()
		{
			OnConnect?.Raise(this);
		}
		public void HandleDisconnect()
		{
			OnDisconnect?.Raise(this);
		}
		public void HandleDataSent()
		{
			OnDataSent?.Raise(this);
		}

		[Header("Events")]
		public GameEventHandler OnConnect;
		public GameEventHandler OnDisconnect;
		public GameEventHandler OnDataReceived;
		public GameEventHandler OnDataSent;
		#endregion

		#region Cleanup
		private void OnDestroy()
		{
			networkingCore?.Dispose();
		}
		#endregion
	}
}
