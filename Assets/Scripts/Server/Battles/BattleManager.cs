using BattleBlast.Server;
using Networking;
using ScriptableSystems;
using System;
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


		[Header("Handled events")]
		public GameEventHandler startTestBattleEvent = new GameEventHandler();


		#region Initialization
		protected override void OnInitialize()
		{
			base.OnInitialize();

			startTestBattleEvent.RegisterListenerOnce(StartTestBattle);
		}
		#endregion


		#region Creating battles
		public Battle CreateBattleFor(PlayerData player1, PlayerData player2, BattleCreationData battleCreationData)
		{
			return new Battle(player1, player2, battleCreationData);
		}

		public async Task<bool> StartBattle(Battle battle)
		{
			ConnectedClient player01 = ServerClientManager.Instance.GetClientForPlayer(battle.Player1);
			ConnectedClient player02 = ServerClientManager.Instance.GetClientForPlayer(battle.Player2);

			LoadBattleRequestData loadBattleRequestData = new LoadBattleRequestData() { battle = battle };

			NetRequest request1 = NetRequest.CreateAndSend(player01.Connection, loadBattleRequestData);
			NetRequest request2 = NetRequest.CreateAndSend(player01.Connection, loadBattleRequestData);

			await Task.WhenAll(request1.WaitForResponse(), request2.WaitForResponse());

			bool player01Loaded = request1.response.GetData<bool>();
			bool player02Loaded = request2.response.GetData<bool>();

			return player01Loaded && player02Loaded;
		}
		#endregion


		#region Cleanup
		public override void Dispose()
		{
			base.Dispose();

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

			bool result = await StartBattle(CreateBattleFor(player01.PlayerData, player02.PlayerData, new BattleCreationData()));
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
	}
}