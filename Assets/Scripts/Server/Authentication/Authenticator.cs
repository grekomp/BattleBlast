using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast.Server
{
	/// <summary>
	/// Handles cretentials verification
	/// </summary>
	public static class Authenticator
	{
		public static bool TryAuthenticatePlayer(Credentials credentials, out PlayerData playerData, out string authToken)
		{
			playerData = BBServer.Instance.Systems.Database.GetPlayerDataByName(credentials.username);
			if (playerData != null && playerData.IsPasswordValid(credentials.password))
			{
				authToken = Guid.NewGuid().ToString();
				return true;
			}

			playerData = null;
			authToken = null;
			return false;
		}
	}
}
