using UnityEngine;
using System.Collections;
using System;

namespace ItSilesiaPlugins {
	[Serializable]
	public class FloatMaterialPropertySetter : SingleMaterialPropertySetter<float> {
		public override SingleMaterialPropertySetter Copy() {
			return new FloatMaterialPropertySetter() { propertyName = propertyName, value = value };
		}

		public override void InitFrom(Material material) {
			value = material.GetFloat(propertyName);
		}

		public override void Set(Material material) {
			material.SetFloat(propertyName, value);
		}
	}
}
