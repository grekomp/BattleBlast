using ScriptableSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BattleBlast
{
	/// <summary>
	/// Gives access to all major server-side functions
	/// </summary>
	[CreateAssetMenu(menuName = "BattleBlast/Server")]
	public class Server : ScriptableSystem<Server>
	{
		[SerializeField]
		protected Database database;
		public static Database Database {
			get {
				return Instance.database;
			}
		}

		[SerializeField]
		protected MatchMaker matchMaker;
		public static MatchMaker MatchMaker {
			get => Instance.matchMaker;
		}

		[SerializeField]
		protected BattleManager battleManager;
		public static BattleManager BattleManager {
			get => Instance.battleManager;
		}

		public static void Dispose()
		{
			MatchMaker.Dispose();
		}
	}
}
