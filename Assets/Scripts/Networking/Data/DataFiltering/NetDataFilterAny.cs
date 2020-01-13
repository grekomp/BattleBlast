using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	public class NetDataFilterAny : NetDataFilter
	{
		public override bool IsValidFor(NetReceivedData receivedData) => true;
	}
}
