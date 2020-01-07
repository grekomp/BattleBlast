using System;
using UnityEngine;

namespace ItSilesiaPlugins {
	[Serializable]
	public class VectorMaterialPropertySetter : SingleMaterialPropertySetter<Vector3> {
		public override SingleMaterialPropertySetter Copy() {
			return new VectorMaterialPropertySetter() { propertyName = propertyName, value = value };
		}

		public override void InitFrom(Material material) {
			value = material.GetVector(propertyName);
		}

		public override void Set(Material material) {
			material.SetVector(propertyName, value);
		}
	}
}
