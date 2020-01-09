namespace Networking
{
	public class ReceivedBroadcastData
	{
		public string senderAddress;
		public int senderPort;

		public ReceivedBroadcastData(string senderAddress, int senderPort)
		{
			this.senderAddress = senderAddress;
			this.senderPort = senderPort;
		}
	}
}