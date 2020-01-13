using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	public class NetDataFilterHost : NetDataFilter
	{
		protected NetHost host;

		public NetDataFilterHost(NetHost host)
		{
			this.host = host;
		}

		public override bool IsValidFor(NetReceivedData receivedData)
		{
			return receivedData.connection.GetHost().Equals(host);
		}
	}
}
