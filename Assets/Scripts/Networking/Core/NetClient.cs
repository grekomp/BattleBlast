using Athanor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Networking
{
	public class NetClient : ScriptableObject
	{
		public enum Status
		{
			Uninitialized,
			InitializedNotConnected,
			SearchingForServer,
			Connecting,
			Connected,
			Error
		}

		[SerializeField] protected NetHost host;
		[SerializeField] protected Status status = Status.Uninitialized;


		#region Properties
		public NetHost Host => host;
		public NetConnection Connection => host.Connections[0];
		public Status ClientStatus => status;
		#endregion


		#region Constructors
		protected NetClient() { }
		public static NetClient CreateClient()
		{
			NetClient client = ScriptableObject.CreateInstance<NetClient>();
			client.host = NetCore.Instance.AddHost(hostName: "NetClient");

			return client;
		}
		#endregion


		#region Connecting to server
		public IEnumerator ConnectToServer(ref NetConnection connection, CancellationToken cancellationToken)
		{
			status = Status.SearchingForServer;

			// Search for server broadcast
			ReceivedBroadcastData broadcastData = null;

			broadcastData = await NetCore.Instance.StartScanningForBroadcast(cancellationToken);

			if (broadcastData == null) return null;

			NetConnection connection = await host.ConnectWithConfirmation(broadcastData.senderAddress, broadcastData.broadcastMessagePort, cancellationToken);
			if (connection != null)
			{
				status = Status.Connected;
				return connection;
			}
			else
			{
				status = Status.InitializedNotConnected;
				return null;
			}
		}

		//protected TaskCompletionSource<ReceivedBroadcastData> broadcastEventReceivedTaskCompletionSource;

		//public async Task<bool> TryConnectToServer(CancellationToken cancellationToken)
		//{
		//	if (status != Status.InitializedNotConnected)
		//	{
		//		Log.Warning(this, $"Cannot try to connect to server because current client status is {status.ToString()}.", this);
		//		return status == Status.Connected;
		//	}

		//	// Start looking for server
		//	status = Status.SearchingForServer;

		//	var task = FindServerBroadcast(cancellationToken);
		//	ReceivedBroadcastData broadcastData = await task;

		//	// Connect to the found server
		//	if (task.Status == TaskStatus.RanToCompletion)
		//	{
		//		connection = await host.ConnectWithConfirmation(broadcastData.senderAddress, broadcastData.broadcastMessagePort);
		//		if (connection != null)
		//		{
		//			connection.OnDisconnectEvent.RegisterListenerOnce(HandleDisconnect);
		//			connection.OnDataEvent.RegisterListenerOnce(HandleDataReceived);

		//			state = ClientState.Connected;
		//			HandleConnect();
		//			return true;
		//		}
		//		else
		//		{
		//			state = ClientState.NotConnected;
		//			return false;
		//		}
		//	}
		//	else
		//	{
		//		state = ClientState.NotConnected;
		//		return false;
		//	}

		//	return false;
		//}
		//protected async Task<ReceivedBroadcastData> FindServerBroadcast(CancellationToken cancellationToken)
		//{
		//	broadcastEventReceivedTaskCompletionSource = new TaskCompletionSource<ReceivedBroadcastData>();
		//	cancellationToken.Register(() => { broadcastEventReceivedTaskCompletionSource.TrySetCanceled(); });
		//	NetCore.Instance.StartScanningForBroadcast();

		//	// Wait for server to be found
		//	ReceivedBroadcastData broadcastData = null;
		//	try
		//	{
		//		broadcastData = await broadcastEventReceivedTaskCompletionSource.Task;
		//	}
		//	finally
		//	{
		//		NetCore.Instance.StopScanningForBroadcast();
		//	}
		//	return broadcastData;
		//}
		#endregion
	}
}
