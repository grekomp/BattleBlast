using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	public class Channel
	{
		public const int Reliable = 0;
		public const int ReliableSequenced = 1;
		public const int ReliableStateUpdate = 2;
		public const int ReliableFragmented = 3;
		public const int Unreliable = 4;
		public const int UnreliableSequenced = 5;
		public const int StateUpdate = 6;
	}
}
