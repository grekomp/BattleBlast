using System;
using System.Collections.Generic;

namespace BattleBlast
{
	[Serializable]
	public class BattleData
	{
		public string id;

		protected PlayerData player1;
		protected PlayerData player2;

		protected BattleCreationData sourceCreationData;

		public List<UnitInstanceData> unitsOnBoard = new List<UnitInstanceData>();

		public PlayerData Player1 { get => player1; }
		public PlayerData Player2 { get => player2; }
		public BattleCreationData SourceCreationData { get => sourceCreationData; }

		public BattleData(PlayerData player1, PlayerData player2, BattleCreationData sourceCreationData)
		{
			this.id = Guid.NewGuid().ToString();
			this.player1 = player1;
			this.player2 = player2;
			this.sourceCreationData = sourceCreationData;
		}
	}
}