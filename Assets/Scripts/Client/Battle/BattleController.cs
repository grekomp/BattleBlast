using Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace BattleBlast
{
	public class BattleController : DontDestroySingleton<BattleController>
	{
		public static readonly string LogTag = nameof(BattleController);


		[Header("Battle controller settings")]
		public BoardController board;
		public BattleUnit battleUnitPrefab;
		public Transform unitSpawnParent;

		[Header("Runtime variables")]
		public BattleData battleData;
		public BattleUnit selectedUnit;

		public BattlePhase battlePhase;
		public DateTime turnStartTime;
		public DateTime turnEndTime;
		public IntReference turnTimeSeconds;
		public DoubleReference turnNormalizedTime;
		public BoolReference enableTimerDisplay;

		public List<BattleUnit> spawnedUnits = new List<BattleUnit>();

		protected DataHandler loadBattleRequestDataHandler;
		protected DataHandler battleCommandStartPlanningPhaseHandler;
		protected DataHandler battleCommandStartActionPhaseHandler;
		protected DataHandler unitActionMoveHandler;
		protected DataHandler unitActionStopHandler;
		protected DataHandler unitActionAttackHandler;
		protected DataHandler unitActionRetaliateHandler;
		protected DataHandler unitActionDieHandler;


		#region Initialization
		protected override void Start()
		{
			base.Start();

			loadBattleRequestDataHandler = DataHandler.New(HandleLoadBattleRequest, new NetDataFilterType(typeof(LoadBattleRequestData)));
			battleCommandStartPlanningPhaseHandler = DataHandler.New(HandleBattleCommandStartPlanningPhase, new NetDataFilterType(typeof(BattleCommandStartPlanningPhase)));
			battleCommandStartActionPhaseHandler = DataHandler.New(HandleBattleCommandStartActionPhase, new NetDataFilterType(typeof(BattleCommandStartActionPhase)));

			unitActionMoveHandler = DataHandler.New(HandleUnitActionMove, new NetDataFilterType(typeof(UnitActionMove)));
			unitActionStopHandler = DataHandler.New(HandleUnitActionStop, new NetDataFilterType(typeof(UnitActionStop)));
			unitActionAttackHandler = DataHandler.New(HandleUnitActionAttack, new NetDataFilterType(typeof(UnitActionAttack)));
			unitActionRetaliateHandler = DataHandler.New(HandleUnitActionRetaliate, new NetDataFilterType(typeof(UnitActionRetaliate)));
			unitActionDieHandler = DataHandler.New(HandleUnitActionDie, new NetDataFilterType(typeof(UnitActionDie)));


			NetDataEventManager.Instance.RegisterHandler(loadBattleRequestDataHandler);
			NetDataEventManager.Instance.RegisterHandler(battleCommandStartPlanningPhaseHandler);
			NetDataEventManager.Instance.RegisterHandler(battleCommandStartActionPhaseHandler);

			NetDataEventManager.Instance.RegisterHandler(unitActionMoveHandler);
			NetDataEventManager.Instance.RegisterHandler(unitActionStopHandler);
			NetDataEventManager.Instance.RegisterHandler(unitActionAttackHandler);
			NetDataEventManager.Instance.RegisterHandler(unitActionRetaliateHandler);
			NetDataEventManager.Instance.RegisterHandler(unitActionDieHandler);
		}
		#endregion


		#region Handling battle commands
		public void HandleLoadBattleRequest(NetReceivedData receivedData)
		{
			LoadBattleRequestData loadBattleRequest = receivedData.data as LoadBattleRequestData;
			battleData = loadBattleRequest.battle;

			ClearSpawnedUnits();

			foreach (var unitInstanceData in battleData.unitsOnBoard)
			{
				SpawnUnitFor(unitInstanceData);
			}

			receivedData.SendResponse(true);
		}
		public void HandleBattleCommandStartPlanningPhase(NetReceivedData receivedData)
		{
			if (receivedData.data is BattleCommandStartPlanningPhase battleCommandStartPlanningPhase)
			{
				turnStartTime = DateTime.Now;
				turnEndTime = DateTime.Now.AddMilliseconds(battleCommandStartPlanningPhase.phaseTime);

				CalculateCurrentTurnTimerValues();
				enableTimerDisplay.Value = true;

				StartPlanningPhase();
				receivedData.SendResponse(true);
			}
		}
		public void HandleBattleCommandStartActionPhase(NetReceivedData receivedData)
		{
			if (receivedData.data is BattleCommandStartActionPhase battleCommandStartActionPhase)
			{
				StartActionPhase();
				receivedData.SendResponse(true);
			}
		}
		#endregion


		#region Managing units
		private void SpawnUnitFor(UnitInstanceData unitInstanceData)
		{
			BattleUnit battleUnit = Instantiate(battleUnitPrefab, unitSpawnParent);
			battleUnit.Initialize(unitInstanceData);
			spawnedUnits.Add(battleUnit);
		}

		private BattleUnit GetBattleUnit(string unitInstanceId)
		{
			return spawnedUnits.Find(u => u.unitInstanceId == unitInstanceId);
		}

		private void ClearSpawnedUnits()
		{
			foreach (var unit in spawnedUnits)
			{
				if (unit) Destroy(unit);
			}
			spawnedUnits.Clear();
		}
		#endregion


		#region Managing flow
		private void StartPlanningPhase()
		{
			Log.Info(LogTag, "Starting planning phase.", this);
			battlePhase = BattlePhase.PlanningPhase;
			enableTimerDisplay.Value = true;

			StartCoroutine(StartTurnTimer());

			//throw new NotImplementedException();
		}
		private void StartActionPhase()
		{
			Log.Info(LogTag, "Starting action phase.", this);
			battlePhase = BattlePhase.ActionPhase;
			enableTimerDisplay.Value = false;
			//throw new NotImplementedException();
		}
		#endregion


		#region Handling unit actions
		public void HandleUnitActionMove(NetReceivedData receivedData)
		{
			if (receivedData.data is UnitActionMove unitAction)
			{
				BattleUnit battleUnit = GetBattleUnit(unitAction.unitInstanceId);

				battleUnit.HandleUnitActionMove(unitAction);

				receivedData.SendResponse(true);
			}
		}

		public async void HandleUnitActionAttack(NetReceivedData receivedData)
		{
			if (receivedData.data is UnitActionAttack unitAction)
			{
				BattleUnit battleUnit = GetBattleUnit(unitAction.unitInstanceId);

				await battleUnit.HandleUnitActionAttack(unitAction);

				BattleUnit targetUnit = GetBattleUnit(unitAction.targetUnitInstanceId);
				targetUnit.SetCount(unitAction.targetRemainingCount);
				targetUnit.SetAttack(unitAction.targetRecalculatedAttack);

				receivedData.SendResponse(true);
			}
		}
		public void HandleUnitActionRetaliate(NetReceivedData receivedData)
		{
			if (receivedData.data is UnitActionRetaliate unitAction)
			{
				BattleUnit battleUnit = GetBattleUnit(unitAction.unitInstanceId);

				battleUnit.HandleUnitActionRetaliate(unitAction);

				BattleUnit targetUnit = GetBattleUnit(unitAction.targetUnitInstanceId);
				targetUnit.SetCount(unitAction.targetRemainingCount);
				targetUnit.SetAttack(unitAction.targetRecalculatedAttack);

				receivedData.SendResponse(true);
			}
		}
		public void HandleUnitActionDie(NetReceivedData receivedData)
		{
			if (receivedData.data is UnitActionDie unitAction)
			{
				BattleUnit battleUnit = GetBattleUnit(unitAction.unitInstanceId);

				battleUnit.HandleUnitActionDie(unitAction);

				receivedData.SendResponse(true);
			}
		}
		public void HandleUnitActionStop(NetReceivedData receivedData)
		{
			if (receivedData.data is UnitActionStop unitAction)
			{
				BattleUnit battleUnit = GetBattleUnit(unitAction.unitInstanceId);

				battleUnit.HandleUnitActionStop(unitAction);

				receivedData.SendResponse(true);
			}
		}
		#endregion


		#region Handling input
		public void EndTurn()
		{

		}

		public void HandleBattleUnitLeftClicked(BattleUnit battleUnit)
		{
			selectedUnit?.Deselect();
			selectedUnit = null;

			if (battleUnit.playerId != NetClient.Instance.PlayerId) return;

			selectedUnit = battleUnit;
			selectedUnit?.Select();
		}
		public void HandleBattleUnitRightClicked(BattleUnit battleUnit)
		{
			if (battlePhase != BattlePhase.PlanningPhase) return;

			if (battleUnit.isFriendlyUnit == false)
			{
				SendUnitOrderMove(battleUnit.tile.x, battleUnit.tile.y);
			}

			if (battleUnit == selectedUnit)
			{
				SendUnitOrderStop(battleUnit);
			}
		}

		public void HandleBoardTileLeftClicked(BoardTile tile)
		{
			selectedUnit?.Deselect();
			selectedUnit = null;
		}

		public void HandleBoardTileRightClicked(BoardTile tile)
		{
			if (selectedUnit)
			{
				SendUnitOrderMove(tile.x, tile.y);
			}
		}

		private async void SendUnitOrderMove(int x, int y)
		{
			UnitOrderMove order = new UnitOrderMove(battleData.id, selectedUnit.unitInstanceId, x, y);

			var request = NetRequest.CreateAndSend(NetClient.Instance.connection, order);
			var response = await request.WaitForResponse();
			if (response.error == null)
			{
				if (response.GetDataOrDefault<bool>())
				{
					selectedUnit.ShowOrderArrow(x, y);
				}
			}
		}
		private async void SendUnitOrderStop(BattleUnit battleUnit)
		{
			UnitOrderStop order = new UnitOrderStop(battleData.id, battleUnit.unitInstanceId);

			var request = NetRequest.CreateAndSend(NetClient.Instance.connection, order);
			var response = await request.WaitForResponse();
			if (response.error == null)
			{
				if (response.GetDataOrDefault<bool>())
				{
					selectedUnit.HideOrderArrow();
				}
			}

		}

		public IEnumerator StartTurnTimer()
		{
			while (turnEndTime > DateTime.Now)
			{
				yield return new WaitForSecondsRealtime(1);
				CalculateCurrentTurnTimerValues();
			}

			turnNormalizedTime.Value = 0;
			turnTimeSeconds.Value = 0;
		}

		private void CalculateCurrentTurnTimerValues()
		{
			turnNormalizedTime.Value = 1.0 - ((DateTime.Now - turnStartTime).TotalMilliseconds / (turnEndTime - turnStartTime).TotalMilliseconds);
			turnTimeSeconds.Value = (int)(turnEndTime - DateTime.Now).TotalSeconds;
		}
		#endregion
	}
}
