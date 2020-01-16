using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitActionMove : UnitAction
	{
		public int fromX;
		public int fromY;
		public int toX;
		public int toY;

		public UnitActionMove(string unitInstanceId, int timingOrder, int fromX, int fromY, int toX, int toY) : base(unitInstanceId, timingOrder)
		{
			this.fromX = fromX;
			this.fromY = fromY;
			this.toX = toX;
			this.toY = toY;
		}
	}
}
