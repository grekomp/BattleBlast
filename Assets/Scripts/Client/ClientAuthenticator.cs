using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace BattleBlast
{
	public class ClientAuthenticator : MonoBehaviour
	{
		public Credentials credentials;

		[ContextMenu("Try authenticate player")]
		public async void TryAuthenticatePlayer()
		{
			bool result = await NetClient.Instance.TryAuthenticate(credentials);
		}
	}
}
