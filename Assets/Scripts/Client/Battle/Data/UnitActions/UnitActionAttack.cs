using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitActionAttack : UnitAction
	{
		public string targetUnitInstanceId;
		public int targetRemainingCount;
		public int targetRecalculatedAttack;

		public UnitActionAttack(string unitInstanceId, int timingOrder, string targetUnitInstanceId, int remainingCount, int targetRecalculatedAttack) : base(unitInstanceId, timingOrder)
		{
			this.targetUnitInstanceId = targetUnitInstanceId;
			this.targetRemainingCount = remainingCount;
			this.targetRecalculatedAttack = targetRecalculatedAttack;
		}
	}
}
