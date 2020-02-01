using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class BattleCommandExecuteUnitActions : BattleCommand
	{
		public List<UnitAction> unitActions;

		public BattleCommandExecuteUnitActions(string battleId, List<UnitAction> unitActions)
		{
			this.battleId = battleId;
			this.unitActions = unitActions;
		}
	}
}
