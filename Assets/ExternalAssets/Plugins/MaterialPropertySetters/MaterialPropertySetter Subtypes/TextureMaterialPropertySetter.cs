using System;
using UnityEngine;

namespace ItSilesiaPlugins {
	[Serializable]
	public class TextureMaterialPropertySetter : SingleMaterialPropertySetter<Texture> {
		public override SingleMaterialPropertySetter Copy() {
			return new TextureMaterialPropertySetter() { propertyName = propertyName, value = value };
		}

		public override void InitFrom(Material material) {
			value = material.GetTexture(propertyName);
		}

		public override void Set(Material material) {
			material.SetTexture(propertyName, value);
		}
	}
}
