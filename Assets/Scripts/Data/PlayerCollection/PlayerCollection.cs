using System;
using System.Collections.Generic;

namespace BattleBlast
{
	/// <summary>
	/// Stores data about units owned by each player
	/// </summary>
	[Serializable]
	public class PlayerCollection : SerializableWideClass
	{
		public List<PlayerCollectionUnitData> playerCollectionUnits = new List<PlayerCollectionUnitData>();

		public List<UnitData> GetAllUnitsInCollection()
		{
			List<UnitData> result = new List<UnitData>();

			foreach (var playerCollectionUnit in playerCollectionUnits)
			{
				var unitData = BBServer.Database.GetUnitData(playerCollectionUnit.unitId);
				if (unitData != null)
				{

				}
			}

			return result;
		}

		public bool IsUnitInCollection(UnitData unitData)
		{
			return playerCollectionUnits.Find(u => u.unitId == unitData.id) != null;
		}
		public int UnitCountInCollection(UnitData unitData)
		{
			throw new NotImplementedException();
		}
		public int UnitTypesCount(UnitData unitData)
		{
			throw new NotImplementedException();
		}
	}
}