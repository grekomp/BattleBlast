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
		[Id] public StringReference id = new StringReference(Guid.NewGuid().ToString());

		public StringReference name = new StringReference();
		public IntReference cost = new IntReference();

		public UnitData()
		{
			id.Value = Guid.NewGuid().ToString();
			name.Value = "Undefined";
			cost.Value = 0;
		}
		public UnitData(string id, string name, int cost)
		{
			this.id.Value = id;
			this.name.Value = name;
			this.cost.Value = cost;
		}
		public UnitData(string name, int cost)
		{
			this.name.Value = name;
			this.cost.Value = cost;
		}

	}
}
