using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all game and player data
/// </summary>
[CreateAssetMenu(menuName = "BattleBlast/Database")]
public class Database : ScriptableObject
{
	[SerializeField]
	private List<PlayerData> playerDataCollection = new List<PlayerData>();

	public List<PlayerData> GetAllPlayerData()
	{
		return playerDataCollection;
	}
	public PlayerData GetPlayerDataById(string id)
	{
		return playerDataCollection.Find(p => p.id == id);
	}
	public PlayerData GetPlayerDataByName(string username)
	{
		return playerDataCollection.Find(p => p.username == username);
	}
}
