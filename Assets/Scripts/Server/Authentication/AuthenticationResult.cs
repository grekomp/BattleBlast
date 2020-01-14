using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class AuthenticationResult
	{
		public bool authenticationSuccessfull = false;
		public string authToken = null;
		public string playerId = null;

		public AuthenticationResult(bool authenticationSuccessfull, string authToken, string playerId)
		{
			this.authenticationSuccessfull = authenticationSuccessfull;
			this.authToken = authToken;
			this.playerId = playerId;
		}
	}
}
