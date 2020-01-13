using System;

namespace Networking
{
	public class NetDataFilterId : NetDataFilter
	{
		protected string id = null;

		public NetDataFilterId(string id)
		{
			this.id = id;
		}

		public override bool IsValidFor(NetReceivedData receivedData)
		{
			return id == receivedData.id;
		}
	}
}
