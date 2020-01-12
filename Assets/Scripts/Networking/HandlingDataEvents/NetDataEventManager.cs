using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	public class NetDataEventManager
	{
		public class RegisteredDataHandler
		{
			public Action<NetReceivedData> registeredAction;
			public Type registeredType;

			public static RegisteredDataHandler New<T>(Action<NetReceivedData> action)
			{
				RegisteredDataHandler registeredDataHandler = new RegisteredDataHandler();
				registeredDataHandler.registeredAction = action;
				registeredDataHandler.registeredType = typeof(T);

				return registeredDataHandler;
			}

			public bool IsValidHandlerFor(NetReceivedData receivedData)
			{
				return registeredType.IsAssignableFrom(receivedData.dataType);
			}
			public void ExecuteHandler(NetReceivedData receivedData)
			{
				registeredAction(receivedData);
			}
		}

		protected List<RegisteredDataHandler> registeredDataHandlers = new List<RegisteredDataHandler>();

		#region Managing handlers
		public void RegisterHandler<T>(Action<NetReceivedData> action)
		{
			registeredDataHandlers.Add(RegisteredDataHandler.New<T>(action));
		}
		public void DeregisterHandler(Action<NetReceivedData> action)
		{
			var registeredDataHandler = registeredDataHandlers.Find(dh => action.Equals(dh.registeredAction));
			if (registeredDataHandler != null)
			{
				registeredDataHandlers.Remove(registeredDataHandler);
			}
		}
		#endregion

		#region Handling data events
		public void HandleDataGameEvent(GameEventData gameEventData)
		{
			if (gameEventData.data is NetReceivedData receivedData) HandleDataEvent(receivedData);
		}
		public void HandleDataEvent(NetReceivedData receivedData)
		{
			List<RegisteredDataHandler> validHandlersForDataType = registeredDataHandlers.FindAll(dh => dh.IsValidHandlerFor(receivedData)).ToList();
			foreach (var handler in validHandlersForDataType)
			{
				handler.ExecuteHandler(receivedData);
			}
		}
		#endregion
	}
}
