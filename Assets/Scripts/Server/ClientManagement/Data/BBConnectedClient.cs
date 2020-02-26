using BattleBlast;
using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleBlast.Server
{
	[Serializable]
	public class BBConnectedClient
	{
		[Header("Variables")]
		public ConnectedClient NetworkingClient;
		public PlayerData PlayerData;
		public string AuthToken;
	}
}
