﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
	[Serializable]
	public class NetConnection
	{
		[SerializeField] [Disabled] protected int id = -1;
		[SerializeField] [Disabled] protected bool connectionConfirmed = false;
		public Func<NetHost> GetHost = () => null;

		protected TaskCompletionSource<bool> connectionConfirmationTaskCompletionSource = new TaskCompletionSource<bool>();

		#region Public properties
		public int Id => id;
		public bool ConnectionConfirmed { get => connectionConfirmed; set => connectionConfirmed = value; }
		#endregion

		public NetConnection(int id, NetHost host)
		{
			this.id = id;
			this.GetHost = () => host;
		}

		public async Task<bool> WaitForConnectionConfirmation(int timeoutMs = -1)
		{
			if (timeoutMs > 0)
			{
				CancellationTokenSource cts = new CancellationTokenSource();
				cts.CancelAfter(timeoutMs);
				cts.Token.Register(() => connectionConfirmationTaskCompletionSource.TrySetCanceled());
			}

			return await connectionConfirmationTaskCompletionSource.Task;
		}
		public void ConfirmConnection()
		{
			connectionConfirmed = true;
			connectionConfirmationTaskCompletionSource.TrySetResult(true);
		}
		public void Disconnect()
		{
			GetHost().Disconnect(this);
			GetHost = () => null;
		}

		#region Sending data
		public NetworkError Send(int channel, byte[] data)
		{
			return NetCore.Instance.Send(GetHost().Id, id, channel, data);
		}
		#endregion

		#region Equals override
		public override bool Equals(object obj)
		{
			if (obj is NetConnection networkingConnection) return Equals(networkingConnection);
			return base.Equals(obj);
		}
		public bool Equals(NetConnection other)
		{
			return other.Id == Id && other.GetHost() == GetHost();
		}
		public override int GetHashCode()
		{
			var hashCode = -816165503;
			hashCode = hashCode * -1521134295 + id.GetHashCode();
			hashCode = hashCode * -1521134295 + GetHost().GetHashCode();
			return hashCode;
		}
		#endregion

	}
}
