namespace Networking
{
	internal class NetDataFilterAnd : NetDataFilter
	{
		private NetDataFilter filter1;
		private NetDataFilter filter2;

		public NetDataFilterAnd(NetDataFilter filter1, NetDataFilter filter2)
		{
			this.filter1 = filter1;
			this.filter2 = filter2;
		}

		public override bool IsValidFor(NetReceivedData receivedData)
		{
			return filter1.IsValidFor(receivedData) && filter2.IsValidFor(receivedData);
		}
	}
}