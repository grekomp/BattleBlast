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
		public bool responseSent = false;

		public NetReceivedData(NetConnection connection, Type dataType, object data, string id = null, bool responseRequired = false)
		{
			this.connection = connection;
			this.dataType = data.GetType();
			this.data = data;
			this.id = id ?? Guid.NewGuid().ToString();
			this.responseRequired = responseRequired;
		}
		public NetReceivedData(NetConnection connection, NetDataPackage dataPackage)
		{
			this.connection = connection;
			id = dataPackage.id;
			dataType = Type.GetType(dataPackage.dataType);
			data = dataPackage.serializedData.Deserialize();
			responseRequired = dataPackage.responseRequired;
		}

		public T GetData<T>()
		{
			return (T)data;
		}

		public void SendResponse(object responseData)
		{
			responseSent = true;
			connection.Send(NetDataPackage.CreateFrom(responseData, id));
		}
	}
}
