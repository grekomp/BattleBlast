using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class ScriptablePlayerData
	{
		public StringReference id = new StringReference();
		public StringReference username = new StringReference();

		public List<string> friendIds = new List<string>();

		public IntReference coins;
		public string collectionId;

		public PlayerData basePlayerData;


		#region Creation
		protected ScriptablePlayerData() { }
		public static ScriptablePlayerData New(PlayerData basePlayerData)
		{
			ScriptablePlayerData scriptablePlayerData = new ScriptablePlayerData();
			scriptablePlayerData.LoadDataFrom(basePlayerData);
			return scriptablePlayerData;
		}
		public void LoadDataFrom(PlayerData basePlayerData)
		{
			id.Value = basePlayerData.id;
			username.Value = basePlayerData.username;

			friendIds = new List<string>(basePlayerData.friendIds);

			coins.Value = basePlayerData.coins;
			collectionId = basePlayerData.collectionId;

			this.basePlayerData = basePlayerData;
		}
		#endregion


	}
}
