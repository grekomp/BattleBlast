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
			public object registeredAction;
			public Type registeredType;

			protected Action<object> handlerDelegate = null;

			public static RegisteredDataHandler New<T>(Action<T> action)
			{
				RegisteredDataHandler registeredDataHandler = new RegisteredDataHandler();
				registeredDataHandler.registeredAction = action;
				registeredDataHandler.registeredType = typeof(T);
				registeredDataHandler.handlerDelegate = (dataObject) => action((T)dataObject);

				return registeredDataHandler;
			}

			public bool IsValidHandlerFor(NetworkingReceivedData receivedData)
			{
				return registeredType.IsAssignableFrom(receivedData.dataType);
			}
			public void ExecuteHandler(NetworkingReceivedData receivedData)
			{
				handlerDelegate.Invoke(receivedData.data);
			}
		}

		protected List<RegisteredDataHandler> registeredDataHandlers = new List<RegisteredDataHandler>();

		#region Managing handlers
		public void RegisterHandler<T>(Action<T> action)
		{
			registeredDataHandlers.Add(RegisteredDataHandler.New(action));
		}
		public void DeregisterHandler<T>(Action<T> action)
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
			if (gameEventData.data is NetworkingReceivedData receivedData) HandleDataEvent(receivedData);
		}
		public void HandleDataEvent(NetworkingReceivedData receivedData)
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
