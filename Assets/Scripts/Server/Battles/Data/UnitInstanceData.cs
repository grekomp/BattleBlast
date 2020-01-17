using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
		public int baseAttack;
		public int minAttack;
		public int count;
		public int baseCount;

		public int x;
		public int y;

		public MoveDirection direction = MoveDirection.None;

		public UnitInstanceData Clone()
		{
			return new UnitInstanceData()
			{
				unitInstanceId = unitInstanceId,
				baseUnitId = baseUnitId,
				playerId = playerId,
				attack = attack,
				baseAttack = baseAttack,
				minAttack = minAttack,
				count = count,
				baseCount = baseCount,
				x = x,
				y = y
			};
		}

		public void RecalculateAttack()
		{
			attack = Mathf.CeilToInt(Mathf.Lerp(minAttack, baseAttack, (float)count / baseCount));
		}
	}
}
