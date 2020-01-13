using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	public class NetDataFilterConnection : NetDataFilter
	{
		protected NetConnection connection;

		public NetDataFilterConnection(NetConnection connection)
		{
			this.connection = connection;
		}

		public override bool IsValidFor(NetReceivedData receivedData)
		{
			return connection == receivedData.connection;
		}
	}
}
