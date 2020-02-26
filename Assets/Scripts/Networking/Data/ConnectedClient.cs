using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Networking
{
	[Serializable]
	public class ConnectedClient
	{
		[SerializeField] protected NetConnection connection;


		#region Public properties
		public NetConnection Connection { get => connection; }
		#endregion


		#region Initialization
		public static ConnectedClient New(NetConnection connection)
		{
			return new ConnectedClient()
			{
				connection = connection,
			};
		}
		#endregion
	}
}
