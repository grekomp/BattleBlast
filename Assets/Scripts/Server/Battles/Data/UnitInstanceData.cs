using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace BattleBlast
{
	[Serializable]
	public class UnitInstanceData
	{
		public string unitInstanceId;
		public string baseUnitId;
		public string playerId;
		public int attack;
		public int count;

		public int x;
		public int y;

		public MoveDirection direction = MoveDirection.None;
	}
}
