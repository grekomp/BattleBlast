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
		public int killedMen;

		public UnitActionRetaliate(string unitInstanceId, int timingOrder, string targetUnitInstanceId, int killedMen) : base(unitInstanceId, timingOrder)
		{
			this.targetUnitInstanceId = targetUnitInstanceId;
			this.killedMen = killedMen;
		}
	}
}
