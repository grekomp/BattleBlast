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
	}
}
