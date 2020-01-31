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

		public int turnTime = 5000;
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
			unitOrderMoveDataHandler = DataHandler.New(HandleUnitOrderMove, new NetDataFilterUnitOrder(battleData.id).And(new NetDataFilterType(typeof(UnitOrderMove))));
			unitOrderStopDataHandler = DataHandler.New(HandleUnitOrderStop, new NetDataFilterUnitOrder(battleData.id).And(new NetDataFilterType(typeof(UnitOrderStop))));
			NetDataEventManager.Instance.RegisterHandler(unitOrderMoveDataHandler);
			NetDataEventManager.Instance.RegisterHandler(unitOrderStopDataHandler);


			// Load players
			LoadBattleRequestData loadBattleRequestData = new LoadBattleRequestData() { battle = battleData };

			NetRequest request1 = NetRequest.CreateAndSend(player1.Connection, loadBattleRequestData, Channel.ReliableFragmented);
			NetRequest request2 = NetRequest.CreateAndSend(player2.Connection, loadBattleRequestData, Channel.ReliableFragmented);

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

				if (battleData.unitsOnBoard.Count == 0) battlePhase = BattlePhase.BattleEnded;
			}
		}
		#endregion


		#region Handling unit orders
		public List<UnitOrder> orders = new List<UnitOrder>();
		public int timingOrder = 0;

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
			UnitInstanceData unit = GetUnitInstanceData(unitOrderMove.unitInstanceId);
			if (unit == null) return false;

			if (Math.Abs(unit.x - unitOrderMove.targetX) + Math.Abs(unit.y - unitOrderMove.targetY) == 1)
			{
				return true;
			}

			return false;
		}
		#endregion


		#region Unit actions
		protected List<UnitAction> ExecuteOrdersAndGenerateActions()
		{
			timingOrder = 0;

			List<UnitAction> actions = new List<UnitAction>();
			ExecuteStopOrders(actions);

			List<UnitOrderMove> remainingOrders = orders.Select(o => o as UnitOrderMove).ToList();
			ExecuteMoveOrders(ref actions, ref remainingOrders);

			orders.Clear();

			return actions;
		}

		protected void ExecuteStopOrders(List<UnitAction> actions)
		{
			foreach (var stopOrder in new List<UnitOrder>(orders).FindAll(o => o is UnitOrderStop))
			{
				orders.Remove(stopOrder);
				UnitInstanceData unit = GetUnitInstanceData(stopOrder.unitInstanceId);
				if (unit == null) return;

				unit.direction = MoveDirection.None;

				actions.Add(new UnitActionStop(stopOrder.unitInstanceId, timingOrder));
			}
		}
		protected void ExecuteMoveOrders(ref List<UnitAction> actions, ref List<UnitOrderMove> remainingOrders)
		{
			bool continueSolvingOrders = true;
			while (continueSolvingOrders && remainingOrders.Count > 0)
			{
				SolveAttackOrders(remainingOrders, actions);

				continueSolvingOrders = SolveUnconflictingOrders(remainingOrders, actions);
				if (continueSolvingOrders) continue;

				continueSolvingOrders = SolveRandomConflictingOrder(remainingOrders, actions);
			}
		}

		private bool SolveRandomConflictingOrder(List<UnitOrderMove> remainingOrders, List<UnitAction> actions)
		{
			List<UnitOrderMove> conflictingOrders = remainingOrders.FindAll(o => IsConflictingOrder(o, remainingOrders)).ToList();
			conflictingOrders.Shuffle();

			if (conflictingOrders.Count > 0)
			{
				ExecuteOrder(conflictingOrders.First(), actions);
				remainingOrders.Remove(conflictingOrders.First());
				return true;
			}

			return false;
		}

		protected void SolveAttackOrders(List<UnitOrderMove> remainingOrders, List<UnitAction> actions)
		{
			List<UnitOrderMove> attackOrders = new List<UnitOrderMove>();

			foreach (var moveOrder in remainingOrders)
			{
				if (GetOrderTargetUnit(moveOrder) != null)
				{
					attackOrders.Add(moveOrder);
				}
			}

			attackOrders.Shuffle();
			foreach (var attackOrder in attackOrders)
			{
				ExecuteOrder(attackOrder, actions);
			}
		}

		protected bool SolveUnconflictingOrders(List<UnitOrderMove> remainingOrders, List<UnitAction> actions)
		{
			bool anyOrdersSolved = false;

			foreach (var moveOrder in new List<UnitOrderMove>(remainingOrders))
			{
				if (IsBlockedOrder(moveOrder, remainingOrders)) continue;
				if (IsConflictingOrder(moveOrder, remainingOrders)) continue;

				ExecuteOrder(moveOrder, actions);
				remainingOrders.Remove(moveOrder);
			}

			return anyOrdersSolved;
		}

		protected void ExecuteOrder(UnitOrderMove order, List<UnitAction> actions)
		{
			UnitInstanceData unit = GetUnitInstanceData(order.unitInstanceId);
			if (unit == null) return;

			UnitInstanceData target = GetOrderTargetUnit(order);

			int xOffset = order.targetX - unit.x;
			int yOffset = order.targetY - unit.y;

			if (xOffset < 0) unit.direction = MoveDirection.Left;
			if (xOffset > 0) unit.direction = MoveDirection.Right;
			if (yOffset < 0) unit.direction = MoveDirection.Up;
			if (yOffset > 0) unit.direction = MoveDirection.Down;

			if (unit == null) return;

			if (target != null)
			{
				if (unit.playerId == target.playerId) return;

				int killedMen = Math.Min(target.count, unit.attack);
				target.count -= killedMen;
				target.RecalculateAttack();

				actions.Add(new UnitActionAttack(unit.unitInstanceId, ++timingOrder, target.unitInstanceId, target.count, target.attack));


				if (target.count <= 0)
				{
					actions.Add(new UnitActionDie(target.unitInstanceId, ++timingOrder));
					battleData.unitsOnBoard.Remove(target);
					return;
				}

				int retaliationKilledMen = Math.Min(unit.count, target.attack);
				unit.count -= retaliationKilledMen;
				unit.RecalculateAttack();

				actions.Add(new UnitActionRetaliate(target.unitInstanceId, ++timingOrder, unit.unitInstanceId, unit.count, unit.attack));

				if (unit.count <= 0)
				{
					actions.Add(new UnitActionDie(unit.unitInstanceId, ++timingOrder));
					battleData.unitsOnBoard.Remove(unit);
				}
			}
			else
			{
				actions.Add(new UnitActionMove(unit.unitInstanceId, ++timingOrder, unit.x, unit.y, order.targetX, order.targetY));

				unit.x = order.targetX;
				unit.y = order.targetY;
			}
		}
		protected bool IsConflictingOrder(UnitOrderMove order, List<UnitOrderMove> remainingOrders)
		{
			UnitInstanceData unit = GetUnitInstanceData(order.unitInstanceId);
			if (unit == null) return false;
			UnitInstanceData target = GetOrderTargetUnit(order);

			if (target != null && target.playerId == unit.playerId) return true;
			if (remainingOrders.Find(o => o != order && o.targetX == order.targetX && o.targetY == order.targetY) != null) return true;

			return false;
		}
		protected bool IsBlockedOrder(UnitOrderMove order, List<UnitOrderMove> remainingOrders)
		{
			UnitInstanceData target = GetOrderTargetUnit(order);

			if (target != null) return true;

			return false;
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

			orders.Clear();
			foreach (var unit in battleData.unitsOnBoard)
			{
				if (unit.direction != MoveDirection.None)
				{
					orders.Add(new UnitOrderMove(battleData.id, unit.unitInstanceId, GetNextX(unit.x, unit.direction), GetNextY(unit.y, unit.direction)));
				}
			}

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
			foreach (var action in actions)
			{
				await SendBoth(action);
			}

			phaseTaskCompletionSource.TrySetResult(true);
		}

		protected async Task SendBattleCommandStartActionPhase()
		{
			await SendBoth(new BattleCommandStartActionPhase() { battleId = battleData.id });
		}
		protected async Task SendBattleCommandStartPlannigPhase()
		{
			await SendBoth(new BattleCommandStartPlanningPhase() { battleId = battleData.id, phaseTime = turnTime });
		}
		protected async Task<bool> SendBoth(object serializableData)
		{
			var request1 = NetRequest.CreateAndSend(player1.Connection, serializableData);
			var request2 = NetRequest.CreateAndSend(player2.Connection, serializableData);
			await Task.WhenAll(request1.WaitForResponse(), request2.WaitForResponse());

			return request1.response.GetDataOrDefault<bool>() && request2.response.GetDataOrDefault<bool>();
		}
		#endregion


		#region Helper methods
		protected int GetNextX(int x, MoveDirection moveDirection)
		{
			if (moveDirection == MoveDirection.Right) return x + 1;
			if (moveDirection == MoveDirection.Left) return x - 1;

			return x;
		}
		protected int GetNextY(int y, MoveDirection moveDirection)
		{
			if (moveDirection == MoveDirection.Up) return y - 1;
			if (moveDirection == MoveDirection.Down) return y + 1;

			return y;
		}

		protected UnitInstanceData GetOrderTargetUnit(UnitOrderMove moveOrder)
		{
			return battleData.unitsOnBoard.Find(u => u.x == moveOrder.targetX && u.y == moveOrder.targetY);
		}
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
