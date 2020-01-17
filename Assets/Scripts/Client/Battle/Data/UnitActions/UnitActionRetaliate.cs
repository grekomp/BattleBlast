using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitActionRetaliate : UnitAction
	{
		public string targetUnitInstanceId;
		public int targetRemainingCount;
		public int targetRecalculatedAttack;

		public UnitActionRetaliate(string unitInstanceId, int timingOrder, string targetUnitInstanceId, int targetRemainingCount, int targetRecalculatedAttack) : base(unitInstanceId, timingOrder)
		{
			this.targetUnitInstanceId = targetUnitInstanceId;
			this.targetRemainingCount = targetRemainingCount;
			this.targetRecalculatedAttack = targetRecalculatedAttack;
		}
	}
}
