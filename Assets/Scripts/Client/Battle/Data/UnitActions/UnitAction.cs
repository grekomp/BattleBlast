using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitAction
	{
		public string unitInstanceId;
		public int timingOrder;

		public UnitAction(string unitInstanceId, int timingOrder)
		{
			this.unitInstanceId = unitInstanceId;
			this.timingOrder = timingOrder;
		}
	}
}
