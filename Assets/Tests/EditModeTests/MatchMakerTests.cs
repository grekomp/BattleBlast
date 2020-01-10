using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BattleBlast;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class MatchMakerTests
	{
		PlayerData testPlayer01 = new PlayerData()
		{
			id = new StringReference(Guid.NewGuid().ToString()),
			username = new StringReference("Test player 01"),
		};
		PlayerData testPlayer02 = new PlayerData()
		{
			id = new StringReference(Guid.NewGuid().ToString()),
			username = new StringReference("Test player 02"),
		};


		MatchMaker.MatchMakerSettings testMatchmakerSettings = new MatchMaker.MatchMakerSettings()
		{
			queueTimeoutLimit = 100
		};

		class InvalidMatchMakingSettings : MatchMakingSettings
		{
			public override bool IsValidMatchFor(MatchMakingSettings matchMakingData)
			{
				return false;
			}
		}

		[SetUp]
		public void SetUp()
		{
			Server.MatchMaker.Dispose();
			Server.MatchMaker.matchMakerSettings = testMatchmakerSettings;
		}

		[UnityTest]
		public IEnumerator Should_AddPlayerToQueue_IfMatchCannotBeFoundInstantly()
		{
			yield return TaskExtensions.RunTaskAsIEnumerator(async () =>
			{
				var task1 = Server.MatchMaker.FindAMatch(testPlayer01, new MatchMakingSettings());
				Assert.That(task1.IsCompleted, Is.False);
				Assert.That(Server.MatchMaker.IsPlayerInQueue(testPlayer01), Is.True);
				await task1;
			});
		}
		[UnityTest]
		public IEnumerator Should_NotAddPlayerToQueue_IfPlayerIsAlreadyInQueue()
		{
			yield return TaskExtensions.RunTaskAsIEnumerator(async () =>
			{
				var task1 = Server.MatchMaker.FindAMatch(testPlayer01, new MatchMakingSettings());
				var task2 = Server.MatchMaker.FindAMatch(testPlayer01, new MatchMakingSettings());
				await task2;

				Assert.That(task1.IsCompleted, Is.False);
				Assert.That(task2.IsCompleted, Is.True);
				Assert.That(task2.Result, Is.Not.Null);
				Assert.That(task2.Result.IsFailed, Is.True);
				Assert.That(task2.Result.status, Is.EqualTo(MatchMakingResult.Status.Error));
			});
		}
		[UnityTest]
		public IEnumerator Should_RemovePlayerFromQueue_IfCancellationWasRequested()
		{
			yield return TaskExtensions.RunTaskAsIEnumerator(async () =>
			{
				var task1 = Server.MatchMaker.FindAMatch(testPlayer01, new MatchMakingSettings());
				Assert.That(Server.MatchMaker.IsPlayerInQueue(testPlayer01), Is.True);
				Assert.That(task1.IsCompleted, Is.False);
				Server.MatchMaker.CancelFindingMatchFor(testPlayer01);
				Assert.That(Server.MatchMaker.IsPlayerInQueue(testPlayer01), Is.False);
				Assert.That(task1.IsCompleted, Is.True);
				Assert.That(task1.Result, Is.Not.Null);
				Assert.That(task1.Result.status, Is.EqualTo(MatchMakingResult.Status.Cancelled));

				// Add another player to queue and verify that he does not get matched with a cancelled player
				var task2 = Server.MatchMaker.FindAMatch(testPlayer02, new MatchMakingSettings());
				Assert.That(Server.MatchMaker.IsPlayerInQueue(testPlayer02), Is.True);
				await task2;
				Server.MatchMaker.CancelFindingMatchFor(testPlayer02);
				Assert.That(Server.MatchMaker.IsPlayerInQueue(testPlayer02), Is.False);
				Assert.That(task2.Result.status, Is.EqualTo(MatchMakingResult.Status.TimedOut));
			});
		}

		[UnityTest]
		public IEnumerator Should_FindAMatch_IfThereAreValidQueuedPlayers()
		{
			yield return TaskExtensions.RunTaskAsIEnumerator(async () =>
			{
				var task1 = Server.MatchMaker.FindAMatch(testPlayer02, new MatchMakingSettings());
				Assert.That(Server.MatchMaker.queue.Count, Is.EqualTo(1));
				Assert.That(Server.MatchMaker.IsPlayerInQueue(testPlayer02), Is.True);

				var task2 = Server.MatchMaker.FindAMatch(testPlayer01, new MatchMakingSettings());
				await task2;

				Assert.That(task2.Result, Is.Not.Null);
				Assert.That(task2.Result.IsSuccessfull, Is.True);

				Assert.That(task1.Result, Is.Not.Null);
				Assert.That(task1.Result.IsSuccessfull, Is.True);
				Assert.That(task1.Result.GetOpponent(testPlayer01), Is.EqualTo(testPlayer02));
				Assert.That(task1.Result.battleId, Is.Not.Null);
				Assert.That(task1.Result.battleId, Is.EqualTo(task2.Result.battleId));

				Assert.That(Server.MatchMaker.queue.Count, Is.Zero);
				Assert.That(Server.MatchMaker.IsPlayerInQueue(testPlayer01), Is.False);
				Assert.That(Server.MatchMaker.IsPlayerInQueue(testPlayer02), Is.False);
			});
		}
		[UnityTest]
		public IEnumerator Should_FailToFindAMatch_AfterTimeout()
		{
			yield return TaskExtensions.RunTaskAsIEnumerator(async () =>
			{
				var task = Server.MatchMaker.FindAMatch(testPlayer01, new MatchMakingSettings());

				//await Task.Delay(testMatchmakerSettings.queueTimeoutLimit / 2);
				Assert.That(task.IsCompleted, Is.False);

				await task;
				Assert.That(task.IsCompleted, Is.True);
				Assert.That(task.Result, Is.Not.Null);
				Assert.That(task.Result.status, Is.EqualTo(MatchMakingResult.Status.TimedOut));
			});
		}
		[UnityTest]
		public IEnumerator Should_FailToFindAMatch_IfOtherPlayersHaveInvalidMatchMakingSetting()
		{
			yield return TaskExtensions.RunTaskAsIEnumerator(async () =>
			{
				var task1 = Server.MatchMaker.FindAMatch(testPlayer01, new InvalidMatchMakingSettings());
				var task2 = Server.MatchMaker.FindAMatch(testPlayer02, new MatchMakingSettings());

				await task2;

				Assert.That(task2.IsCompleted, Is.True);
				Assert.That(task2.Result.status, Is.EqualTo(MatchMakingResult.Status.TimedOut));
			});
		}

		[TearDown]
		public void TearDown()
		{
			Server.Dispose();
		}
	}
}
