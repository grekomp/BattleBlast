using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleBlast
{
	/// <summary>
	/// Stores all game and player data
	/// </summary>
	[CreateAssetMenu(menuName = "BattleBlast/Database")]
	public class Database : ScriptableObject
	{
		[SerializeField]
		private List<PlayerData> playerDataCollection = new List<PlayerData>();

		[SerializeField]
		private List<UnitData> unitDataCollection = new List<UnitData>();

		public List<PlayerData> GetAllPlayerData()
		{
			return playerDataCollection;
		}
		public PlayerData GetPlayerDataById(string id)
		{
			return playerDataCollection.Find(p => p.id == id);
		}

		public UnitData GetUnitData(string unitId)
		{
			return unitDataCollection.Find(u => u.id == unitId);
		}

		public PlayerData GetPlayerDataByName(string username)
		{
			return playerDataCollection.Find(p => p.username == username);
		}

		public void Initialize()
		{
			//throw new NotImplementedException();
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}
	}
}
