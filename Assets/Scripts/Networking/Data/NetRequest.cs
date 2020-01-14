﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
	[Serializable]
	public class NetRequest
	{
		public string id = Guid.NewGuid().ToString();
		public NetConnection connection = null;
		public NetReceivedData response = null;


		protected DataHandler responseHandler;
		protected TaskCompletionSource<NetReceivedData> responseTaskCompletionSource = new TaskCompletionSource<NetReceivedData>();


		#region Creation
		public static NetRequest CreateAndSend(NetConnection connection, object serializableData, int channel = Channel.ReliableSequenced)
		{
			return CreateAndSend(connection, NetDataPackage.CreateFrom(serializableData, responseRequired: true), channel);
		}
		public static NetRequest CreateAndSend(NetConnection connection, NetDataPackage dataPackage, int channel = Channel.ReliableSequenced)
		{
			NetRequest netRequest = new NetRequest()
			{
				id = dataPackage.id,
				connection = connection,
			};

			dataPackage.responseRequired = true;
			netRequest.RegisterResponseHandler();
			connection.Send(dataPackage, channel);

			connection.OnDisconnectEvent.RegisterListener(netRequest.Dispose);

			return netRequest;
		}

		#endregion


		#region Handling response
		protected void RegisterResponseHandler()
		{
			responseHandler = DataHandler.New(SetResponse, new NetDataFilterConnection(connection).And(new NetDataFilterId(id)), true);
			NetDataEventManager.Instance.RegisterHandler(responseHandler);
		}
		protected void DeregisterResponseHandler()
		{
			NetDataEventManager.Instance.DeregisterHandler(responseHandler);
		}

		public void SetResponse(NetReceivedData receivedData)
		{
			response = receivedData;
			responseTaskCompletionSource.TrySetResult(response);
		}

		public async Task<NetReceivedData> WaitForResponse()
		{
			return await responseTaskCompletionSource.Task;
		}
		protected void CancelWaitingForResponse()
		{
			responseTaskCompletionSource.TrySetCanceled();
		}
		#endregion


		#region Dispose
		public void Dispose()
		{
			CancelWaitingForResponse();
			DeregisterResponseHandler();
		}
		#endregion
	}
}
