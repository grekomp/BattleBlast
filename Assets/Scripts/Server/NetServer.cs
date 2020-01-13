using Networking;
using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace BattleBlast.Server
{
	[CreateAssetMenu(menuName = "BattleBlast/Networking/NetServer")]
	public class NetServer : ScriptableSystem<NetServer>
	{
		private static readonly string LogTag = nameof(NetServer);

		[Header("Runtime Variables")]
		public NetHost host;

		protected ServerClientManager serverClientManager = new ServerClientManager();



		#region Initialization
		protected override void OnInitialize()
		{
			host = NetCore.Instance.AddHost();

			// Register event listeners
			host.OnConnectEvent.RegisterListenerOnce(HandleConnect);
			host.OnDisconnectEvent.RegisterListenerOnce(HandleDisconnect);
			host.OnDataEvent.RegisterListenerOnce(HandleDataReceived);
			host.OnBroadcastEvent.RegisterListenerOnce(HandleBroadcastEvent);
		}
		#endregion


		#region Starting and Stopping Server
		[ContextMenu(nameof(StartServer))]
		public void StartServer()
		{
			Log.Info(LogTag, "Starting server...", this);
			Initialize();
			NetCore.Instance.StartBroadcastDiscovery(host.Port);
			Log.Info(LogTag, $"Server started on HostId: {host.Id}, Port: {host.Port}.", this);
		}
		[ContextMenu(nameof(StopServer))]
		public void StopServer()
		{
			Log.Info(LogTag, "Stopping server...", this);
			NetCore.Instance.StopBroadcastDiscovery();
			Log.Info(LogTag, "Server stopped.", this);
		}
		#endregion


		#region Handling Data
		/// <summary>
		/// Sends provided serializableData object to the client.
		/// </summary>
		/// <param name="serializableData">Data object to send. Must be a serializable class.</param>
		/// <returns>Whether the operation succeeded.</returns>
		public bool SendData(NetConnection connection, object serializableData, int channel = Channel.ReliableSequenced)
		{
			if (connection.ConnectionConfirmed == false) return false;

			var dataPackage = NetDataPackage.CreateFrom(serializableData);
			var error = connection.Send(channel, dataPackage.SerializeToByteArray());

			if (error == UnityEngine.Networking.NetworkError.Ok)
			{
				OnDataSent?.Raise(this, serializableData);
			}

			return error == UnityEngine.Networking.NetworkError.Ok;
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
			//if (state != ClientState.LookingForServer) return;

			//if (gameEventData.data is ReceivedBroadcastData receivedBroadcastData)
			//{
			//	if (broadcastEventReceivedTaskCompletionSource != null)
			//	{
			//		broadcastEventReceivedTaskCompletionSource.TrySetResult(receivedBroadcastData);
			//	}
			//}
		}

		[Header("Events")]
		public GameEventHandler OnConnect;
		public GameEventHandler OnDisconnect;
		public GameEventHandler OnDataReceived;
		public GameEventHandler OnDataSent;
		#endregion

		#region Cleanup
		public override void Dispose()
		{
			if (NetCore.InstanceExists == false) return;

			NetCore.Instance.RemoveHost(host);

			// Deregister event listeners
			NetCore.Instance.OnConnectEvent.DeregisterListener(HandleConnect);
			NetCore.Instance.OnDisconnectEvent.DeregisterListener(HandleDisconnect);
			NetCore.Instance.OnDataReceivedEvent.DeregisterListener(HandleDataReceived);
			NetCore.Instance.OnBroadcastEvent.DeregisterListener(HandleBroadcastEvent);

			base.Dispose();
		}
		#endregion
	}
}
