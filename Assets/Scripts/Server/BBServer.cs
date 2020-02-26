using Athanor;
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
	[CreateAssetMenu(menuName = "BattleBlast/Networking/BBServer")]
	public class BBServer : ScriptableSystem<BBServer>
	{
		private static readonly string LogTag = nameof(BBServer);

		[Header("Server options")]
		[SerializeField] private BoolReference autoStart = new BoolReference(false);
		[SerializeField] private ServerSystems systems = new ServerSystems();

		[Header("Runtime Variables")]
		public NetHost host;

		[Header("Event handlers")]
		public GameEventHandler startServerEvent = new GameEventHandler();
		public GameEventHandler stopServerEvent = new GameEventHandler();

		[Header("Raised events")]
		public GameEventHandler OnConnect;
		public GameEventHandler OnDisconnect;
		public GameEventHandler OnDataReceived;
		public GameEventHandler OnDataSent;


		#region Public properties
		public ServerSystems Systems => systems;
		#endregion


		#region Initialization
		protected override void OnInitialize()
		{
			base.OnInitialize();

			// Register event listeners
			startServerEvent.RegisterListenerOnce(StartServer);
			stopServerEvent.RegisterListenerOnce(StopServer);
		}

		protected override void OnStart()
		{
			base.OnStart();

			// Autostart 
			if (autoStart) StartServer();
		}
		#endregion


		#region Starting and Stopping Server
		[ContextMenu(nameof(StartServer))]
		public void StartServer()
		{
			Log.Info(LogTag, "Starting server...", this);
			// Initialize server
			Initialize();

			// Add host
			host = NetCore.Instance.AddHost(hostName: "Server host");

			// Initialize systems
			systems.Initialize();

			// Register host event listeners
			host.OnConnectEvent.RegisterListenerOnce(HandleConnect);
			host.OnDisconnectEvent.RegisterListenerOnce(HandleDisconnect);
			host.OnDataEvent.RegisterListenerOnce(HandleDataReceived);
			host.OnBroadcastEvent.RegisterListenerOnce(HandleBroadcastEvent);

			// Start broadcasting
			NetCore.Instance.StartBroadcastDiscovery(host.Port);
			Log.Info(LogTag, $"Server started on Host: {host}.", this);
		}
		[ContextMenu(nameof(StopServer))]
		public void StopServer()
		{
			Log.Info(LogTag, "Stopping server...", this);
			NetCore.Instance.StopBroadcastDiscovery();
			Dispose();
			Log.Info(LogTag, "Server stopped.", this);
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

		}
		#endregion

		#region Cleanup
		public override void Dispose()
		{
			if (NetCore.InstanceExists == false) return;

			systems.Dispose();
			NetCore.Instance.RemoveHost(host);

			base.Dispose();
		}
		#endregion
	}
}
