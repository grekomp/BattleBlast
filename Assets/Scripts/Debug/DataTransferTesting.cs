
using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace BattleBlast
{
	public class DataTransferTesting : MonoBehaviour
	{
		public int hostId = 0;
		public int connectionId = 0;
		[Space]
		public string testMessage = "Test message";
		public UnityEngine.Object testObject;

		[ContextMenu(nameof(SendTestString))]
		public void SendTestString()
		{
			NetDataPackage networkingDataPackage = NetDataPackage.CreateFrom(testMessage);
			NetCore.Instance.Send(hostId, connectionId, Channel.ReliableSequenced, networkingDataPackage.SerializeToByteArray());
		}
		[ContextMenu(nameof(SendTestObject))]
		public void SendTestObject()
		{
			NetDataPackage networkingDataPackage = NetDataPackage.CreateFrom(testObject);
			NetCore.Instance.Send(hostId, connectionId, Channel.ReliableSequenced, networkingDataPackage.SerializeToByteArray());
		}
	}
}
