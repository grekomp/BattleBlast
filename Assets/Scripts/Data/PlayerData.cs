using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleBlast
{
	[Serializable]
	public class PlayerData : SerializableWideClass
	{
		[Id] public string id = Guid.NewGuid().ToString();
		public string username;
		public string password;

		public List<string> friendIds = new List<string>();

		public int coins;
		public string collectionId;

		public PlayerData()
		{
			id = Guid.NewGuid().ToString();
		}

		public bool IsPasswordValid(string potentialPassword)
		{
			return password == potentialPassword;
		}
	}
}
