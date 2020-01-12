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
		protected Action<NetReceivedData> testDataEventHandler;

		[ContextMenu("Register test data event handler")]
		public void RegisterTestDataEventHandler()
		{
			testDataEventHandler = (NetReceivedData receivedData) => Log.D(receivedData.data);
			NetClient.Instance.DataEventManager.RegisterHandler<System.String>(testDataEventHandler);
		}
		[ContextMenu("Deregister test data event handler")]
		public void DeregisterTestDataEventHandler()
		{
			NetClient.Instance.DataEventManager.DeregisterHandler(testDataEventHandler);
		}
	}
}
