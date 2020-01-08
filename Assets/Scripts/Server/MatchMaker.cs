using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleBlast
{

	/// <summary>
	/// Handles finding opponents and creating games
	/// </summary>
	[CreateAssetMenu(menuName = "BattleBlast/Systems/MatchMaker")]
	public class MatchMaker : ScriptableSystem
	{
		#region Inner classes
		[Serializable]
		public class MatchMakerSettings
		{
			public int queueTimeoutLimit = 360000;
		}

		[Serializable]
		public class QueuedPlayer
		{
			public PlayerData player;
			public MatchMakingSettings matchMakingData;

			public TaskCompletionSource<MatchMakingResult> matchFoundTaskCompletionSource;
			public CancellationTokenSource timeoutCancellationTokenSource;

			public QueuedPlayer(PlayerData player, MatchMakingSettings matchMakingData)
			{
				this.player = player;
				this.matchMakingData = matchMakingData;
				matchFoundTaskCompletionSource = new TaskCompletionSource<MatchMakingResult>();
			}

			public bool IsValidMatchFor(QueuedPlayer otherPlayer)
			{
				if (otherPlayer == this) return false;
				if (matchMakingData.IsValidMatchFor(otherPlayer.matchMakingData) == false) return false;
				if (otherPlayer.matchMakingData.IsValidMatchFor(matchMakingData) == false) return false;

				return true;
			}

			public void MatchFound()
			{
				CancelTimeout();
			}

			public void SetTimeout(int queueTimeoutLimit, Action<QueuedPlayer> timeoutPlayerAction)
			{
				timeoutCancellationTokenSource = new CancellationTokenSource();

				Task.Run(async () =>
				{
					await Task.Delay(queueTimeoutLimit);
					timeoutPlayerAction(this);
				}, timeoutCancellationTokenSource.Token);
			}
			private void CancelTimeout()
			{
				timeoutCancellationTokenSource.Cancel();
			}
		}
		#endregion

		[Header("Match Maker Options")]
		public MatchMakerSettings settings;
		public List<QueuedPlayer> queue = new List<QueuedPlayer>();

		#region Public methods
		public async Task<MatchMakingResult> FindAMatch(PlayerData player, MatchMakingSettings matchMakingData)
		{
			QueuedPlayer newQueueEntry = CreatePlayerQueueEntryFor(player, matchMakingData);

			if (IsPlayerInQueue(player))
			{
				return new MatchMakingResult(player, null, null, MatchMakingResult.Status.Error);
			}

			if (TryMatchingWithCurrentQueue(newQueueEntry, out MatchMakingResult result))
			{
				return result;
			}
			else
			{
				AddPlayerToQueue(newQueueEntry);

				return await newQueueEntry.matchFoundTaskCompletionSource.Task;
			}
		}
		public bool IsPlayerInQueue(PlayerData player)
		{
			return GetQueueEntryFor(player) != null;
		}
		public void CancelFindingMatchFor(PlayerData player)
		{
			QueuedPlayer queuedPlayer = GetQueueEntryFor(player);
			if (queuedPlayer != null) queuedPlayer.matchFoundTaskCompletionSource?.SetResult(MatchMakingResult.GetCancelledResult(player));

			RemoveQueueEntry(queuedPlayer);
		}
		#endregion

		#region Matching players
		private bool TryMatchingWithCurrentQueue(QueuedPlayer newQueueEntry, out MatchMakingResult result)
		{
			QueuedPlayer matchingPlayer = queue.Find(qp => qp.IsValidMatchFor(newQueueEntry));

			if (matchingPlayer != null)
			{
				SetMatchFoundFor(matchingPlayer);

				var battleId = CreateNewBattle(matchingPlayer.player, newQueueEntry.player, new BattleCreationData());
				result = new MatchMakingResult(newQueueEntry.player, matchingPlayer.player, battleId, MatchMakingResult.Status.MatchFound);

				matchingPlayer.matchFoundTaskCompletionSource.SetResult(result);
				return true;
			}

			result = null;
			return false;
		}
		private void SetMatchFoundFor(QueuedPlayer player)
		{
			player.MatchFound();

			RemoveQueueEntry(player);
		}

		private Battle CreateNewBattle(PlayerData player1, PlayerData player2, BattleCreationData battleCreationData)
		{
			return Server.BattleManager.CreateBattleFor(player1, player2, battleCreationData);
		}
		#endregion

		#region Queue helpers
		private QueuedPlayer CreatePlayerQueueEntryFor(PlayerData player, MatchMakingSettings matchMakingData)
		{
			return new QueuedPlayer(player, matchMakingData);
		}
		private void AddPlayerToQueue(QueuedPlayer player)
		{
			player.SetTimeout(settings.queueTimeoutLimit, TimeoutQueuedPlayer);
			queue.Add(player);
		}
		private QueuedPlayer GetQueueEntryFor(PlayerData player)
		{
			return queue.Find(qp => qp.player == player);
		}
		private void TimeoutQueuedPlayer(QueuedPlayer queuedPlayer)
		{
			RemoveQueueEntry(queuedPlayer);
			queuedPlayer.matchFoundTaskCompletionSource.SetResult(MatchMakingResult.GetTimeoutResult(queuedPlayer.player));
		}
		private void RemoveQueueEntry(QueuedPlayer matchingPlayer)
		{
			queue.Remove(matchingPlayer);
		}
		#endregion

		#region Tear down
		public void Dispose()
		{
			CancelAllQueuedPlayers();
			queue.Clear();
		}
		private void CancelAllQueuedPlayers()
		{
			foreach (var queuedPlayer in new List<QueuedPlayer>(queue))
			{
				if (queuedPlayer != null)
					CancelFindingMatchFor(queuedPlayer.player);
			}
		}
		#endregion
	}
}
