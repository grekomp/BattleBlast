using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleBlast;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class PlayerCollectionTests
	{
		UnitData testUnitData01 = new UnitData("unit1", "Unit 01", 100);
		UnitData testUnitData02 = new UnitData("unit2", "Unit 02", 100);
		UnitData testUnitData03 = new UnitData("unit3", "Unit 03", 100);
		UnitData testUnitData04 = new UnitData("unit4", "Unit 04", 100);

		UnitData testUnitDataNotInCollection01 = new UnitData()
		{
			id = new StringReference("unitNotInCollection 01"),
			name = new StringReference("Unit not in collection 01")
		};

		PlayerCollection testCollection = null;

		[SetUp]
		public void SetUp()
		{
			testCollection = new PlayerCollection()
			{
				playerCollectionUnits = new List<PlayerCollectionUnitData>()
				{
					new PlayerCollectionUnitData(){ unitId = "unit1", count = 2 },
					new PlayerCollectionUnitData(){ unitId = "unit2", count = 1 },
					new PlayerCollectionUnitData(){ unitId = "unit3", count = 4 },
					new PlayerCollectionUnitData(){ unitId = "unit4", count = 2 },
				}
			};
		}

		[Test]
		public void Should_ReturnTrue_IfUnitIsInCollection()
		{
			Assert.That(testCollection.IsUnitInCollection(testUnitData01), Is.True);
			Assert.That(testCollection.IsUnitInCollection(testUnitDataNotInCollection01), Is.False);
		}

		[Test]
		public void Should_ReturnACollectionOfUnitDataCorrespondingToUnitsInCollection_WhenRequested()
		{
			List<UnitData> unitDataList = new List<UnitData>()
			{
				testUnitData01,
				testUnitData02,
				testUnitData03,
				testUnitData04
			};

			Assert.That(testCollection.GetAllUnitsInCollection().SequenceEqual(unitDataList));
		}
	}
}
