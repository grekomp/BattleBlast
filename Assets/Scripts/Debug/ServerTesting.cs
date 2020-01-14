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
		protected DataHandler dataHandler;

		[ContextMenu("Register test data event handler")]
		public void RegisterTestDataEventHandler()
		{
			dataHandler = DataHandler.New((NetReceivedData receivedData) => Log.D(receivedData.data), new NetDataFilterAny());
			NetDataEventManager.Instance.RegisterHandler(dataHandler);
		}
		[ContextMenu("Register test request handler")]
		public void RegisterTestRequestHandler()
		{
			dataHandler = DataHandler.New((NetReceivedData receivedData) =>
			{
				Log.D(receivedData.data);
				if (receivedData.responseRequired) receivedData.SendResponse("This is a correct response");
			}, new NetDataFilterAny());
			NetDataEventManager.Instance.RegisterHandler(dataHandler);
		}
		[ContextMenu("Deregister test data event handler")]
		public void DeregisterTestDataEventHandler()
		{
			NetDataEventManager.Instance.DeregisterHandler(dataHandler);
		}
	}
}
