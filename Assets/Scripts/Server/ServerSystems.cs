using ScriptableSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BattleBlast.Server
{
	/// <summary>
	/// Gives access to all major server-side functions
	/// </summary>
	[Serializable]
	public class ServerSystems
	{
		[SerializeField] protected Database database;
		[SerializeField] protected MatchMaker matchMaker;
		[SerializeField] protected BattleManager battleManager;
		[SerializeField] protected ServerClientManager clientManager;

		#region Public properties
		public Database Database => database;
		public MatchMaker MatchMaker => matchMaker;
		public BattleManager BattleManager => battleManager;
		public ServerClientManager ClientManager => clientManager;
		#endregion


		#region Initialization
		public void Initialize()
		{
			database?.Initialize();
			matchMaker?.Initialize();
			battleManager?.Initialize();
			clientManager?.Initialize();
		}
		#endregion


		#region Cleanup
		public void Dispose()
		{
			database?.Dispose();
			matchMaker?.Dispose();
			battleManager?.Dispose();
			clientManager?.Dispose();
		}
		#endregion
	}
}
