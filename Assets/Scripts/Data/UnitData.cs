using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class UnitData
	{
		public string name;
		public string id = Guid.NewGuid().ToString();
		public int cost = 10;

		public int maxAttack = 5;
		public int count = 100;

		public UnitData()
		{
			id = Guid.NewGuid().ToString();
			name = "Undefined";
			cost = 0;
		}
		public UnitData(string id, string name, int cost)
		{
			this.id = id;
			this.name = name;
			this.cost = cost;
		}
		public UnitData(string name, int cost)
		{
			this.name = name;
			this.cost = cost;
		}

	}
}
