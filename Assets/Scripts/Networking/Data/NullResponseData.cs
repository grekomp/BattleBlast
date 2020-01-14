using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	[Serializable]
	public class NullResponseData
	{
		public string message;

		public NullResponseData(string message = "Request was not handled correctly.")
		{
			this.message = message;
		}
	}
}
