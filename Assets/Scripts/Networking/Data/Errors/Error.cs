using System;

namespace Networking
{
	[Serializable]
	public class Error
	{
		public string message = "No message";

		public Error() { }
		public Error(string message)
		{
			this.message = message;
		}

		public override string ToString() => $"{this.GetType().Name}: {message}";
	}
}