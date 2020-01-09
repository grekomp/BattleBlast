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
		public int connectionId;
		public Type dataType;
		public object data;

		public NetworkingReceivedData(int connectionId, Type dataType, object data)
		{
			this.connectionId = connectionId;
			this.dataType = data.GetType();
			this.data = data;
		}
		public NetworkingReceivedData(int connectionId, NetworkingDataPackage dataPackage)
		{
			this.connectionId = connectionId;
			dataType = Type.GetType(dataPackage.dataType);
			data = dataPackage.serializedData.Deserialize();
		}

		public T GetData<T>()
		{
			return (T)data;
		}
	}
}
