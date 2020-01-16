using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	public class UnitActionDie : UnitAction
	{
		public UnitActionDie(string unitInstanceId, int timingOrder) : base(unitInstanceId, timingOrder) { }
	}
}
