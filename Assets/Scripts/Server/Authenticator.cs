using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	/// <summary>
	/// Handles cretentials verification
	/// </summary>
	public class Authenticator
	{
		public bool TryAuthenticatePlayer(Credentials credentials, out PlayerData playerData)
		{
			playerData = Server.Database.GetPlayerDataByName(credentials.username);
			if (playerData != null && playerData.IsPasswordValid(credentials.password))
			{
				return true;
			}

			playerData = null;
			return false;
		}
	}
}
