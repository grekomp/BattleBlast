using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using Utils;

namespace BattleBlast.Server
{
	[Serializable]
	public class BattleSession
	{
		public static readonly string LogTag = nameof(BattleSession);


		public BattleData battleData;

		public ConnectedClient player1;
		public ConnectedClient player2;

		public int turnTime = 20000;
		public int turnEndingTime = 3000;
		public DateTime nextTurnTime;

		public BattlePhase battlePhase = BattlePhase.NotStarted;
		public TaskCompletionSource<object> phaseTaskCompletionSource = new TaskCompletionSource<object>();
		public CancellationTokenSource phaseTimerCancellationTokenSource = new CancellationTokenSource();


		protected DataHandler unitOrderMoveDataHandler;
		protected DataHandler unitOrderStopDataHandler;


		#region Initialization
		protected BattleSession()
		{ }
		public static BattleSession New(BattleData battleData)
		{
			BattleSession battleSession = new BattleSession();
			battleSession.battleData = battleData;
			battleSession.player1 = ServerClientManager.Instance.GetClientForPlayer(battleData.Player1);
			battleSession.player2 = ServerClientManager.Instance.GetClientForPlayer(battleData.Player2);
			battleSession.Start();
			return battleSession;
		}

		public async void Start()
		{
			battlePhase = BattlePhase.Starting;

			// Register data handlers
			unitOrderMoveDataHandler = DataHandler.New(HandleUnitOrderMove, new NetDataFilterUnitOrder(battleData.id));
			unitOrderStopDataHandler = DataHandler.New(HandleUnitOrderStop, new NetDataFilterUnitOrder(battleData.id));
			NetDataEventManager.Instance.RegisterHandler(unitOrderMoveDataHandler);
			NetDataEventManager.Instance.RegisterHandler(unitOrderStopDataHandler);


			// Load players
			LoadBattleRequestData loadBattleRequestData = new LoadBattleRequestData() { battle = battleData };

			NetRequest request1 = NetRequest.CreateAndSend(player1.Connection, loadBattleRequestData);
			NetRequest request2 = NetRequest.CreateAndSend(player2.Connection, loadBattleRequestData);

			await Task.WhenAll(request1.WaitForResponse(), request2.WaitForResponse());

			bool player01Loaded = request1.response.GetDataOrDefault<bool>();
			bool player02Loaded = request2.response.GetDataOrDefault<bool>();

			if (player01Loaded == false || player02Loaded == false)
			{
				Log.Error(LogTag, $"Failed to start BattleSession for BattleData: {battleData}.");
				return;
			}

			// Start battle phases loop
			while (battlePhase != BattlePhase.BattleEnded)
			{
				StartPlanningPhase();
				await WaitForEndOfPhase();

				StartActionPhase();
				await WaitForEndOfPhase();
			}
		}
		#endregion


		#region Handling unit orders
		public List<UnitOrder> orders = new List<UnitOrder>();

		public void HandleUnitOrderMove(NetReceivedData receivedData)
		{
			if (receivedData.data is UnitOrderMove unitOrder)
			{
				if (IsValidOrder(unitOrder))
				{
					var existingOrder = orders.Find(u => u.unitInstanceId == unitOrder.unitInstanceId);
					if (existingOrder != null) orders.Remove(existingOrder);

					orders.Add(unitOrder);
					receivedData.SendResponse(true);
				}
				else
				{
					receivedData.SendResponse(false);
				}
			}
		}
		public void HandleUnitOrderStop(NetReceivedData receivedData)
		{
			if (receivedData.data is UnitOrderStop unitOrder)
			{
				var existingOrder = orders.Find(u => u.unitInstanceId == unitOrder.unitInstanceId);
				if (existingOrder != null) orders.Remove(existingOrder);

				orders.Add(unitOrder);
				receivedData.SendResponse(true);
			}
		}


		protected bool IsValidOrder(UnitOrderMove unitOrderMove)
		{
			throw new NotImplementedException();
		}
		#endregion


		#region Unit actions
		protected List<UnitAction> ExecuteOrdersAndGenerateActions()
		{
			List<UnitAction> actions = new List<UnitAction>();
			List<UnitOrder> remainingOrders = new List<UnitOrder>(orders);

			// Execute stop orders
			foreach (var stopOrder in orders.FindAll(o => o is UnitOrderStop))
			{
				UnitInstanceData unitInstanceData = GetUnitInstanceData(stopOrder.unitInstanceId);
				unitInstanceData.direction = MoveDirection.None;

				actions.Add(new UnitActionStop() { unitInstanceId = stopOrder.unitInstanceId });
				remainingOrders.Remove(stopOrder);
			}

			// Execute move orders
			throw new NotImplementedException();

			return actions;
		}
		#endregion


		#region Battle flow
		public void PlayerEndedTurn(PlayerData player)
		{
			phaseTimerCancellationTokenSource.Cancel();
		}

		protected async void StartPlanningPhase()
		{
			phaseTaskCompletionSource = new TaskCompletionSource<object>();
			phaseTimerCancellationTokenSource = new CancellationTokenSource();

			battlePhase = BattlePhase.PlanningPhase;
			await SendBattleCommandStartPlannigPhase();

			nextTurnTime = DateTime.Now.AddMilliseconds(turnTime);

			try
			{
				await Task.Delay(turnTime, phaseTimerCancellationTokenSource.Token);
			}
			catch (TaskCanceledException)
			{
				int turnTimerRemainingMs = (int)(DateTime.Now - nextTurnTime).TotalMilliseconds;
				await Task.Delay(Math.Min(turnTimerRemainingMs, turnEndingTime));
			}

			phaseTaskCompletionSource.TrySetResult(true);
		}
		protected async void StartActionPhase()
		{
			phaseTaskCompletionSource = new TaskCompletionSource<object>();

			battlePhase = BattlePhase.ActionPhase;
			await SendBattleCommandStartActionPhase();

			List<UnitAction> actions = ExecuteOrdersAndGenerateActions();
			List<Task> actionTasks = new List<Task>();
			foreach (var action in actions)
			{
				actionTasks.Add(SendBoth(action));
			}

			// Execute all orders and generate unitActions
			// Send all actions and wait for players to finish executing them

			throw new NotImplementedException();
		}

		protected async Task SendBattleCommandStartActionPhase()
		{
			await SendBoth(new BattleCommandStartActionPhase() { battleId = battleData.id });
		}
		protected async Task SendBattleCommandStartPlannigPhase()
		{
			await SendBoth(new BattleCommandStartPlanningPhase() { battleId = battleData.id });
		}
		protected async Task<bool> SendBoth(object serializableData)
		{
			var request1 = NetRequest.CreateAndSend(player1.Connection, serializableData);
			var request2 = NetRequest.CreateAndSend(player2.Connection, serializableData);
			await Task.WhenAll(request1.WaitForResponse(), request2.WaitForResponse());

			return request1.response.GetDataOrDefault<bool>() && request2.response.GetDataOrDefault<bool>();
		}
		#endregion


		#region Data access helpers
		protected UnitInstanceData GetUnitInstanceData(string unitInstanceId)
		{
			return battleData.unitsOnBoard.Find(u => u.unitInstanceId == unitInstanceId);
		}
		#endregion


		#region Timing
		public async Task WaitForEndOfPhase()
		{
			await phaseTaskCompletionSource.Task;
		}
		#endregion


		#region Cleanup
		public void EndBattle()
		{
			NetDataEventManager.Instance.DeregisterHandler(unitOrderMoveDataHandler);
			NetDataEventManager.Instance.DeregisterHandler(unitOrderStopDataHandler);
		}
		#endregion
	}
}
