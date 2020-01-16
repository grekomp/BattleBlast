using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	public class NetDataFilterUnitOrder : NetDataFilter
	{
		public readonly string battleId;

		public NetDataFilterUnitOrder(string battleId)
		{
			this.battleId = battleId;
		}

		public override bool IsValidFor(NetReceivedData receivedData)
		{
			if (receivedData.data is UnitOrder unitOrder)
			{
				return unitOrder.battleId == battleId;
			}

			return false;
		}
	}
}
