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
		[Id] public StringReference id;
		public StringReference username;

		[SerializeField]
		protected string password;

		public List<string> friendIds;
		public IntReference coins;

		public PlayerCollection collection;

		public bool IsPasswordValid(string potentialPassword)
		{
			return password == potentialPassword;
		}

		// TODO: Status
	}
}
