using UnityEngine;
using System.Collections;
using System;

namespace ItSilesiaPlugins {
	[Serializable]
	public class ColorMaterialPropertySetter : SingleMaterialPropertySetter<Color> {
		public override SingleMaterialPropertySetter Copy() {
			return new ColorMaterialPropertySetter() { propertyName = propertyName, value = value };
		}

		public override void InitFrom(Material material) {
			value = material.GetColor(propertyName);
		}

		public override void Set(Material material) {
			material.SetColor(propertyName, value);
		}
	}
}