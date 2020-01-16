
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitOrderMove : UnitOrder
	{
		public int targetX;
		public int targetY;

		public UnitOrderMove(string battleId, string unitInstanceId, int targetX, int targetY) : base(battleId, unitInstanceId)
		{
			this.targetX = targetX;
			this.targetY = targetY;
		}
	}
}
