using ScriptableSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Networking
{
	[CreateAssetMenu(menuName = "Systems/Networking/Networking Client")]
	public class NetworkingClient : ScriptableSystem<NetworkingClient>
	{
		[Header("Networking Client Options")]
		[SerializeField] protected NetworkingCore networkingCore;

		[Header("Runtime Variables")]
		[Disabled] protected int connectionId = -1;

		#region Initialization
		protected override void OnInitialize()
		{
			base.OnInitialize();

			if (networkingCore == null) networkingCore = ScriptableObject.CreateInstance<NetworkingCore>();
		}
		#endregion

		#region Managing Connection
		/// <summary>
		/// Attempts to automatically connect to any available server.
		/// </summary>
		/// <returns></returns>
		public async Task ConnectToServer()
		{


			throw new NotImplementedException();
		}
		public void DisconnectFromServer()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Handling Data
		public bool SendData(object serializableData)
		{
			var dataPackage = NetworkingDataPackage.CreateFrom(serializableData);
			var error = networkingCore.Send(connectionId, Channel.reliable, dataPackage.SerializeToByteArray());
			return error == UnityEngine.Networking.NetworkError.Ok;
		}
		public void RegisterDataHandler<T>(Action<T> action)
		{
			throw new NotImplementedException();
		}

		protected void DataReceivedHandler(NetworkingDataPackage dataPackage)
		{

		}
		#endregion

		#region Handling Events
		[Header("Events")]
		public GameEventHandler OnConnect;
		public GameEventHandler OnDisconnect;
		public GameEventHandler OnDataReceived;
		public GameEventHandler OnDataSent;
		#endregion
	}
}
