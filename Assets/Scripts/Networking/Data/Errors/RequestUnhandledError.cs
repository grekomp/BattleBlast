using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	[Serializable]
	public class RequestUnhandledError : Error
	{
		public RequestUnhandledError() { }
		public RequestUnhandledError(string message) : base(message) { }
	}
}
