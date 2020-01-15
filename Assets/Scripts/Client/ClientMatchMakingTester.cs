using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace BattleBlast
{
	public class ClientMatchMakingTester : MonoBehaviour
	{
		public string playerId;

		[ContextMenu("Send match request")]
		public async void SendMatchRequest()
		{
			var request = NetRequest.CreateAndSend(NetClient.Instance.connection, new MatchMakingRequest(playerId, new MatchMakingSettings()));
			NetReceivedData response = await request.WaitForResponse();
			Log.D(response.data);
		}
	}
}
