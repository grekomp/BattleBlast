using System;

namespace Networking
{
	public class NetDataFilterType : NetDataFilter
	{
		protected Type filteredType = null;

		public NetDataFilterType(Type filteredType)
		{
			this.filteredType = filteredType;
		}

		public override bool IsValidFor(NetReceivedData receivedData)
		{
			return filteredType.IsAssignableFrom(receivedData.dataType);
		}
	}
}
