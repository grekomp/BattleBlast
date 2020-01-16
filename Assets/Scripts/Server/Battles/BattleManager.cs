using BattleBlast.Server;
using Networking;
using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace BattleBlast
{
	[CreateAssetMenu(menuName = "BattleBlast/Systems/BattleManager")]
	public class BattleManager : ScriptableSystem
	{
		public readonly static string LogTag = nameof(BattleManager);
		public static BattleManager Instance => NetServer.Instance.Systems.BattleManager;

		public List<UnitInstanceData> testBattleUnits = new List<UnitInstanceData>();

		[Header("Runtime variables")]
		public List<BattleSession> battleSessions = new List<BattleSession>();


		[Header("Handled events")]
		public GameEventHandler startTestBattleEvent = new GameEventHandler();


		protected DataHandler unitOrderMoveHandler;


		#region Initialization
		protected override void OnInitialize()
		{
			base.OnInitialize();

			unitOrderMoveHandler = DataHandler.New(HandleUnitOrderMove, new NetDataFilterType(typeof(UnitOrderMove)));
			NetDataEventManager.Instance.RegisterHandler(unitOrderMoveHandler);

			startTestBattleEvent.RegisterListenerOnce(StartTestBattle);
		}
		#endregion


		#region Creating battles
		public BattleData CreateBattleFor(PlayerData player1, PlayerData player2, BattleCreationData battleCreationData)
		{
			return new BattleData(player1, player2, battleCreationData);
		}

		public async Task<bool> StartBattle(BattleData battle)
		{
			BattleSession battleSession = BattleSession.New(battle);
			return true;
		}
		#endregion


		#region Cleanup
		public override void Dispose()
		{
			base.Dispose();

			NetDataEventManager.Instance.DeregisterHandler(unitOrderMoveHandler);
			startTestBattleEvent.DeregisterListener(StartTestBattle);
		}
		#endregion


		#region Debug controls
		[ContextMenu(nameof(StartTestBattle))]
		public async void StartTestBattle()
		{
			Log.Info(LogTag, "Starting test battle...", this);

			// Check if there are at least two players connected and authenticated
			if (NetServer.Instance.Systems.ClientManager.ConnectedClients.Count < 2)
			{
				Log.Warning(LogTag, "Failed starting test battle, there are not enough connected clients.", this);
				return;
			}

			ConnectedClient player01 = NetServer.Instance.Systems.ClientManager.ConnectedClients[0];
			ConnectedClient player02 = NetServer.Instance.Systems.ClientManager.ConnectedClients[1];

			BattleData battle = CreateBattleFor(player01.PlayerData, player02.PlayerData, new BattleCreationData());
			battle.unitsOnBoard = testBattleUnits;

			bool result = await StartBattle(battle);
			if (result)
			{
				Log.Info(LogTag, "Test Battle started successfully.", this);
			}
			else
			{
				Log.Warning(LogTag, "Failed to start test battle.", this);
			}
		}
		#endregion


		#region Validating orders
		public void HandleUnitOrderMove(NetReceivedData netReceivedData)
		{
			if (netReceivedData.data is UnitOrderMove unitOrderMove)
			{
				netReceivedData.SendResponse(true);
			}
		}
		#endregion
	}
}