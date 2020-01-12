using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Networking
{
	[CreateAssetMenu(menuName = "BattleBlast/Networking/NetServer")]
	public class NetServer : ScriptableSystem<NetServer>
	{
		private static readonly string LogTag = nameof(NetServer);

		[Header("Runtime Variables")]
		public NetHost host;

		protected NetDataEventManager dataEventManager = new NetDataEventManager();


		#region Public properties
		public NetDataEventManager DataEventManager { get => dataEventManager; }
		#endregion


		#region Initialization
		protected override void OnInitialize()
		{
			host = NetCore.Instance.AddHost();
			host.OnDataEvent.RegisterListenerOnce(dataEventManager.HandleDataGameEvent);

			// Register event listeners
			host.OnConnectEvent.RegisterListenerOnce(HandleConnect);
			host.OnDisconnectEvent.RegisterListenerOnce(HandleDisconnect);
			host.OnDataEvent.RegisterListenerOnce(HandleDataReceived);
			host.OnBroadcastEvent.RegisterListenerOnce(HandleBroadcastEvent);
		}
		#endregion


		#region Starting and Stopping Server
		public void StartServer()
		{
			NetCore.Instance.StartBroadcastDiscovery(host.Port);
		}
		public void StopServer()
		{
			NetCore.Instance.StopBroadcastDiscovery();
		}
		#endregion


		#region Clients

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

			var dataPackage = NetworkingDataPackage.CreateFrom(serializableData);
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
			Log.D(LogTag, "Dispose", this);

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
