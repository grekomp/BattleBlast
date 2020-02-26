using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Networking
{
	public class NetReceivedData
	{
		public string id;
		public NetConnection connection;
		public Type dataType;
		public object data;
		public bool responseRequired = false;
		public bool requestHandled = false;
		public Error error;

		public NetReceivedData(NetConnection connection, NetDataPackage dataPackage)
		{
			this.connection = connection;
			id = dataPackage.id;
			dataType = dataPackage.dataType != null ? Type.GetType(dataPackage.dataType) : null;
			data = dataPackage.serializedData.Deserialize();
			responseRequired = dataPackage.responseRequired;
			error = (Error)dataPackage.serializedError.Deserialize();
		}


		#region Retrieving data
		public T GetData<T>()
		{
			return (T)data;
		}
		public T GetDataOrDefault<T>()
		{
			if (data is T dataT)
			{
				return dataT;
			}

			return default;
		}
		#endregion


		#region Sending response
		public void SendResponse(object responseData, Error error = null)
		{
			requestHandled = true;
			connection.Send(NetDataPackage.CreateFrom(responseData, id, error: error));
		}
		#endregion
	}
}
