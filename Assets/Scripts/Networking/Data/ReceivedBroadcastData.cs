namespace Networking
{
	public class ReceivedBroadcastData
	{
		public NetHost host;
		public string senderAddress;
		public int senderPort;
		public int broadcastMessagePort;

		public ReceivedBroadcastData(NetHost host, string senderAddress, int senderPort, int broadcastMessagePort)
		{
			this.host = host;
			this.senderAddress = senderAddress;
			this.senderPort = senderPort;
			this.broadcastMessagePort = broadcastMessagePort;
		}
	}
}