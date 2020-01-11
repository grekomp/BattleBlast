using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Networking
{
	public class NetworkingReceivedData
	{
		public NetConnection connection;
		public Type dataType;
		public object data;


		public NetworkingReceivedData(NetConnection connection, Type dataType, object data)
		{
			this.connection = connection;
			this.dataType = data.GetType();
			this.data = data;
		}
		public NetworkingReceivedData(NetConnection connection, NetworkingDataPackage dataPackage)
		{
			this.connection = connection;
			dataType = Type.GetType(dataPackage.dataType);
			data = dataPackage.serializedData.Deserialize();
		}

		public T GetData<T>()
		{
			return (T)data;
		}
	}
}
