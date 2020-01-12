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
	public class ClientTesting : MonoBehaviour
	{
		public NetClient client;

		protected Action<System.String> testDataEventHandler;

		[ContextMenu("Register test data event handler")]
		public void RegisterTestDataEventHandler()
		{
			testDataEventHandler = (System.String s) => Log.D(s);
			NetClient.Instance.DataEventManager.RegisterHandler(testDataEventHandler);
		}
		[ContextMenu("Deregister test data event handler")]
		public void DeregisterTestDataEventHandler()
		{
			NetClient.Instance.DataEventManager.DeregisterHandler(testDataEventHandler);
		}
	}
}
