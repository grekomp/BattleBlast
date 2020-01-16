using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleBlast
{
	public class BattleUnitTester : MonoBehaviour
	{
		public BattleUnit unit;
		public int targetX;
		public int targetY;
		public string targetUnitInstanceId;
		public int menKilled;

		public MoveDirection direction;

		[ContextMenu("Execute move action")]
		public void ExecuteMoveAction()
		{
			unit?.HandleUnitActionMove(new UnitActionMove() { fromX = unit.tile.x, fromY = unit.tile.y, toX = targetX, toY = targetY });
		}

		[ContextMenu("Show order preview arrow")]
		public void ShowOrderPreviewArrow()
		{
			unit?.ShowOrderArrow(direction);
		}

		[ContextMenu("Hide Arrow")]
		public void HideArrow()
		{
			unit?.HideOrderArrow();
		}
	}
}
