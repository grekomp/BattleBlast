using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitOrderStop : UnitOrder
	{
		public UnitOrderStop(string battleId, string unitInstanceId) : base(battleId, unitInstanceId) { }
	}
}
