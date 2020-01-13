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

		public NetReceivedData(NetConnection connection, Type dataType, object data)
		{
			this.connection = connection;
			this.dataType = data.GetType();
			this.data = data;
		}
		public NetReceivedData(NetConnection connection, NetDataPackage dataPackage)
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
