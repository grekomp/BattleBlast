using ScriptableSystems;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleBlast/Systems/BattleManager")]
public class BattleManager : ScriptableSystem
{
	public Battle CreateBattleFor(PlayerData player1, PlayerData player2, BattleCreationData battleCreationData)
	{
		return new Battle(player1, player2, battleCreationData);
	}
}