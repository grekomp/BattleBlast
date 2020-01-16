using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	/// <summary>
	/// A base class for all commands sent by the server to the client battle controllers.
	/// </summary>
	[Serializable]
	public class BattleCommand
	{
		public string battleId;
	}
}
