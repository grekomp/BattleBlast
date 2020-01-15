using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	public class NetDataEventManager : DontDestroySingleton<NetDataEventManager>
	{
		protected List<DataHandler> registeredDataHandlers = new List<DataHandler>();

		#region Managing handlers
		public void RegisterHandler(DataHandler dataHandler)
		{
			registeredDataHandlers.Add(dataHandler);
		}
		public DataHandler RegisterHandler(Action<NetReceivedData> action, NetDataFilter dataFilter, bool isOneShotHandler = false)
		{
			DataHandler dataHandler = DataHandler.New(action, dataFilter, isOneShotHandler);
			RegisterHandler(dataHandler);
			return dataHandler;
		}
		public void DeregisterHandler(DataHandler dataHandler)
		{
			registeredDataHandlers.Remove(dataHandler);
		}
		#endregion

		#region Handling data events
		public void HandleDataGameEvent(GameEventData gameEventData)
		{
			if (gameEventData.data is NetReceivedData receivedData) HandleDataEvent(receivedData);
		}
		public void HandleDataEvent(NetReceivedData receivedData)
		{
			List<DataHandler> validHandlersForDataType = registeredDataHandlers.FindAll(dh => dh.IsValidHandlerFor(receivedData)).ToList();

			foreach (var handler in validHandlersForDataType)
			{
				handler.ExecuteHandler(receivedData);
				if (handler.IsOneShotHandler) DeregisterHandler(handler);
			}

			if (receivedData.responseRequired && receivedData.requestHandled == false)
			{
				receivedData.SendResponse(new NullResponseData());
			}
		}
		#endregion
	}
}
