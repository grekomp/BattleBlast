using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class MatchMakingRequest
	{
		public string playerId;
		public MatchMakingSettings matchMakingSettings;

		public MatchMakingRequest(string playerId, MatchMakingSettings matchMakingSettings)
		{
			this.playerId = playerId;
			this.matchMakingSettings = matchMakingSettings;
		}
	}
}
