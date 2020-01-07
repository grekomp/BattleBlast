using System;

public class Battle
{
	[Id] public StringReference id = new StringReference(Guid.NewGuid().ToString());

	private PlayerData player1;
	private PlayerData player2;

	private BattleCreationData sourceCreationData;

	public PlayerData Player1 { get => player1; }
	public PlayerData Player2 { get => player2; }
	public BattleCreationData SourceCreationData { get => sourceCreationData; }

	public Battle(PlayerData player1, PlayerData player2, BattleCreationData sourceCreationData)
	{
		this.player1 = player1;
		this.player2 = player2;
		this.sourceCreationData = sourceCreationData;
	}
}