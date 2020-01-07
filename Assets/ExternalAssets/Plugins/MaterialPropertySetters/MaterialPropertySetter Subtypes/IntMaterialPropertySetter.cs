using System;
using UnityEngine;

namespace ItSilesiaPlugins {
	[Serializable]
	public class IntMaterialPropertySetter : SingleMaterialPropertySetter<int> {
		public override SingleMaterialPropertySetter Copy() {
			return new IntMaterialPropertySetter() { propertyName = propertyName, value = value };
		}

		public override void InitFrom(Material material) {
			value = material.GetInt(propertyName);
		}

		public override void Set(Material material) {
			material.SetInt(propertyName, value);
		}
	}
}
