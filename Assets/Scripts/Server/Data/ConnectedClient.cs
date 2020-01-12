using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast.Server
{
	public class ConnectedClient
	{
		protected NetConnection connection;
		protected PlayerData playerData;
		protected string authToken;

		#region Public properties
		public NetConnection Connection { get => connection; }
		public PlayerData PlayerData { get => playerData; }
		public string AuthToken { get => authToken; }
		#endregion

		public static ConnectedClient New(NetConnection connection, PlayerData playerData, string authToken)
		{
			return new ConnectedClient()
			{
				connection = connection,
				playerData = playerData,
				authToken = authToken
			};
		}
	}
}
