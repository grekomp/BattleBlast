using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleBlast
{
	public class BattleController : DontDestroySingleton<BattleController>
	{
		[Header("Battle controller settings")]
		public BoardController board;

		[Header("Runtime variables")]
		public BattleData battleData;
		public BattleUnit selectedUnit;


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
