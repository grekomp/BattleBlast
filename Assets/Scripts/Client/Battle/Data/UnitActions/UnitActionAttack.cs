using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitActionAttack : UnitAction
	{
		public string targetUnitInstanceId;
		public int killedMen;
	}
}
