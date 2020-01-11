namespace Networking
{
	public class ReceivedBroadcastData
	{
		public NetHost host;
		public string senderAddress;
		public int senderPort;

		public ReceivedBroadcastData(NetHost host, string senderAddress, int senderPort)
		{
			this.host = host;
			this.senderAddress = senderAddress;
			this.senderPort = senderPort;
		}
	}
}