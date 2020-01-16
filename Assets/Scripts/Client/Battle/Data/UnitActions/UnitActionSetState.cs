using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	public class UnitActionSetState : UnitAction
	{
		public int attack;
		public int count;

		public int x;
		public int y;

		public UnitActionSetState(string unitInstanceId, int timingOrder, int attack, int count, int x, int y) : base(unitInstanceId, timingOrder)
		{
			this.attack = attack;
			this.count = count;
			this.x = x;
			this.y = y;
		}
	}
}
