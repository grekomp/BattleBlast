using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	[Serializable]
	public class NetRequest
	{
		public string id = Guid.NewGuid().ToString();
		public NetDataPackage dataPackage;

		public static NetRequest CreateFrom(object serializableData)
		{
			return new NetRequest()
			{
				dataPackage = NetDataPackage.CreateFrom(serializableData)
			};
		}
	}
}
