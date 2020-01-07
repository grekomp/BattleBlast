public static class OrderParser {
	public static readonly string Delimiter = ":";

	public static Order ParseMessage(string message, out string data) {
		int delimiterIndex = message.IndexOf(Delimiter);
		if (delimiterIndex < 0) {
			data = "";
			return Order.None;
		}
		if (delimiterIndex < message.Length - 1) {
			data = message.Substring(delimiterIndex + 1, message.Length - (delimiterIndex + 1));
		}
		else {
			data = string.Empty;
		}
		string orderString = message.Substring(0, delimiterIndex);
		return (Order)System.Enum.Parse(typeof(Order), orderString, true);
	}

	public static byte[] CreateMessage(Order order, string data) {
		return Serializator.GetBytes(string.Format("{0}{1}{2}", order.ToString(), Delimiter, data));
	}
}
