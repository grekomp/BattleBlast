using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace BattleBlast.Server
{
	public class ServerTesting : MonoBehaviour
	{
		protected Action<NetReceivedData> testDataEventHandler;

		[ContextMenu("Register test data event handler")]
		public void RegisterTestDataEventHandler()
		{
			testDataEventHandler = (NetReceivedData receivedData) => Log.D(receivedData.data);
			NetServer.Instance.DataEventManager.RegisterHandler<System.String>(testDataEventHandler);
		}
		[ContextMenu("Deregister test data event handler")]
		public void DeregisterTestDataEventHandler()
		{
			NetServer.Instance.DataEventManager.DeregisterHandler(testDataEventHandler);
		}
	}
}
