using System;

namespace Networking
{
	public class NetDataFilterInverse : NetDataFilter
	{
		protected NetDataFilter baseFilter;

		public NetDataFilterInverse(NetDataFilter baseFilter)
		{
			this.baseFilter = baseFilter;
		}

		public override bool IsValidFor(NetReceivedData receivedData)
		{
			return !baseFilter.IsValidFor(receivedData);
		}
	}
}
