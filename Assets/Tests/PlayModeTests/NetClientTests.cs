using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Networking;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class NetClientTests
	{
		[SetUp]
		public void SetUp()
		{
			NetCore.Instance.Initialize();
			NetCore.Instance.StopScanningForBroadcast();
			NetCore.Instance.StopBroadcastDiscovery();
		}

		[TearDown]
		public void TearDown()
		{
			NetCore.Instance.Dispose();
		}


		[Test]
		public void ConnectToServer_ShouldCancelAfterSpecifiedTimeout_IfThereAreNoAvailableServers()
		{
			Task.Run(async () =>
			{
				NetClient client = NetClient.CreateClient();

				Debug.Log("ok");
				var connection = await client.ConnectToServer(2000.GetCancellationToken());
				Debug.Log("ok 2");

				Assert.That(connection, Is.Null);

			}).GetAwaiter().GetResult();
		}


		[UnityTest]
		public async Task ConnectToServer_ShouldFindServer_IfThereIsAnAvailableServer()
		{
			NetClient client = NetClient.CreateClient();
			NetServer server = NetServer.CreateServer();
			server.Start();

			var connection = await client.ConnectToServer(1.GetCancellationToken());

			Assert.That(connection, Is.Not.Null);
		}
	}
}
