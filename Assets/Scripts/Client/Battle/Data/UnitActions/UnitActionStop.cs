using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitActionStop : UnitAction
	{
		public UnitActionStop(string unitInstanceId, int timingOrder) : base(unitInstanceId, timingOrder) { }
	}
}
