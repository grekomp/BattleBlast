
namespace Networking
{
	public abstract class NetDataFilter
	{
		public abstract bool IsValidFor(NetReceivedData receivedData);

		#region Chained filters
		public NetDataFilter Inverse => new NetDataFilterInverse(this);
		public NetDataFilter And(NetDataFilter nextFilter) => new NetDataFilterAnd(this, nextFilter);
		public NetDataFilter Or(NetDataFilter nextFilter) => new NetDataFilterOr(this, nextFilter);
		#endregion
	}
}
