using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitOrder
	{
		public string battleId;
		public string unitInstanceId;

		public UnitOrder(string battleId, string unitInstanceId)
		{
			this.battleId = battleId;
			this.unitInstanceId = unitInstanceId;
		}
	}
}
