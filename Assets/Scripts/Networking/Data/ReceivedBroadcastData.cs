namespace Networking
{
	public class ReceivedBroadcastData
	{
		public NetConnection connection;
		public string senderAddress;
		public int senderPort;

		public ReceivedBroadcastData(NetConnection connection, string senderAddress, int senderPort)
		{
			this.connection = connection;
			this.senderAddress = senderAddress;
			this.senderPort = senderPort;
		}
	}
}