using Networking;
using System;
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

		public List<BattleUnit> spawnedUnits = new List<BattleUnit>();

		protected DataHandler loadBattleRequestDataHandler;
		protected DataHandler battleCommandStartPlanningPhaseHandler;
		protected DataHandler battleCommandStartActionPhaseHandler;

		#region Initialization
		protected override void Start()
		{
			base.Start();

			loadBattleRequestDataHandler = DataHandler.New(HandleLoadBattleRequest, new NetDataFilterType(typeof(LoadBattleRequestData)));
			battleCommandStartPlanningPhaseHandler = DataHandler.New(HandleBattleCommandStartPlanningPhase, new NetDataFilterType(typeof(BattleCommandStartPlanningPhase)));
			battleCommandStartActionPhaseHandler = DataHandler.New(HandleBattleCommandStartActionPhase, new NetDataFilterType(typeof(BattleCommandStartActionPhase)));
			NetDataEventManager.Instance.RegisterHandler(loadBattleRequestDataHandler);
			NetDataEventManager.Instance.RegisterHandler(battleCommandStartPlanningPhaseHandler);
			NetDataEventManager.Instance.RegisterHandler(battleCommandStartActionPhaseHandler);
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
			//throw new NotImplementedException();
		}
		private void StartActionPhase()
		{
			Log.Info(LogTag, "Starting action phase.", this);
			//throw new NotImplementedException();
		}
		#endregion


		#region Handling input
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
			throw new NotImplementedException();
		}

		public void HandleBoardTileLeftClicked(BoardTile tile)
		{
			selectedUnit?.Deselect();
			selectedUnit = null;
		}

		public async void HandleBoardTileRightClicked(BoardTile tile)
		{
			if (selectedUnit)
			{
				UnitOrderMove order = new UnitOrderMove(battleData.id, selectedUnit.unitInstanceId, tile.x, tile.y);

				var request = NetRequest.CreateAndSend(NetClient.Instance.connection, order);
				var response = await request.WaitForResponse();
				if (response.error == null)
				{
					if (response.GetDataOrDefault<bool>())
					{
						selectedUnit.ShowOrderArrow(tile);
					}
				}
			}
		}
		#endregion
	}
}
