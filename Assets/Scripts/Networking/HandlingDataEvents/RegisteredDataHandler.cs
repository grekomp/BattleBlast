using System;

namespace Networking
{
	public class DataHandler
	{
		protected Action<NetReceivedData> registeredAction;
		protected NetDataFilter dataFilter;
		protected bool isOneShotHandler = false;


		public Action<NetReceivedData> RegisteredAction => registeredAction;
		public NetDataFilter DataFilter => dataFilter;
		public bool IsOneShotHandler => isOneShotHandler;


		public static DataHandler New(Action<NetReceivedData> action, NetDataFilter dataFilter, bool isOneShotHandler = false)
		{
			DataHandler registeredDataHandler = new DataHandler();
			registeredDataHandler.registeredAction = action;
			registeredDataHandler.isOneShotHandler = isOneShotHandler;

			return registeredDataHandler;
		}

		public bool IsValidHandlerFor(NetReceivedData receivedData)
		{
			return dataFilter.IsValidFor(receivedData);
		}
		public void ExecuteHandler(NetReceivedData receivedData)
		{
			registeredAction(receivedData);
		}
	}
}