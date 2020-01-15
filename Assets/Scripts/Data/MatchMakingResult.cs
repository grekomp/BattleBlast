using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBlast
{
	[Serializable]
	public class MatchMakingResult
	{
		public enum Status
		{
			MatchFound,
			TimedOut,
			Cancelled,
			Error
		}

		public bool IsSuccessfull { get => status == Status.MatchFound; }
		public bool IsFailed { get => !IsSuccessfull; }

		public PlayerData player1;
		public PlayerData player2;
		public string battleId;

		public Status status;

		public MatchMakingResult(PlayerData player1, PlayerData player2, Battle battle, Status status)
		{
			this.player1 = player1;
			this.player2 = player2;
			this.battleId = battle.id;
			this.status = status;
		}

		public static MatchMakingResult GetTimeoutResult(PlayerData player)
		{
			return new MatchMakingResult(player, null, null, Status.TimedOut);
		}
		public static MatchMakingResult GetCancelledResult(PlayerData player)
		{
			return new MatchMakingResult(player, null, null, Status.Cancelled);
		}

		public PlayerData GetOpponent(PlayerData player)
		{
			if (player == player1) return player2;
			if (player == player2) return player1;

			throw new ArgumentException($"{nameof(GetOpponent)}: Player {player} was not part of the match.");
		}
	}
}
