using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Networking;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests
{
	public class NetServerTests
	{
		[UnityTest]
		public async Task StartServer_Should_StartBroadcastingServerPortAndSetCorrectStatus()
		{
			NetServer server = NetServer.CreateServer();

			Assert.That(server.netHost, Is.Not.Null);
			Assert.That(server.ServerStatus, Is.EqualTo(NetServer.Status.InitializedNotRunning));
			Assert.That(server.ClientManager.connectedClients.Count, Is.EqualTo(0));
			Assert.That(NetCore.Instance.IsBroadcasting, Is.False);

			server.Start();
			Assert.That(server.ServerStatus, Is.EqualTo(NetServer.Status.Running));
			Assert.That(NetCore.Instance.IsBroadcasting, Is.True);

			server.Stop();
			await Task.Delay(50);
			Assert.That(server.ServerStatus, Is.EqualTo(NetServer.Status.Stopped));
			Assert.That(NetCore.Instance.IsBroadcasting, Is.False);
		}
		[Test]
		public void HandleClientConnected_Should_UpdateConnectedClients()
		{
			NetServer server = NetServer.CreateServer();
			server.Start();

			NetClient client = NetClient.CreateClient();
			//client.SearchForServer();
		}

		[TearDown]
		public void Teardown()
		{
			NetCore.Instance.Dispose();
		}
	}
}
