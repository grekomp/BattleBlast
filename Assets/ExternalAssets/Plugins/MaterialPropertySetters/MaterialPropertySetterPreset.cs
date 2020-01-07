using UnityEngine;
using System.Collections;

namespace ItSilesiaPlugins {
	public abstract class MaterialPropertySetterPreset : ScriptableObject {
		public abstract void Set(Material material);
		public abstract SingleMaterialPropertySetter GetSetter();
	}

	public class MaterialPropertySetterPreset<T, K> : MaterialPropertySetterPreset where K : SingleMaterialPropertySetter<T> {
		public K materialPropertySetter;

		public override void Set(Material material) {
			materialPropertySetter?.Set(material);
		}

		public override SingleMaterialPropertySetter GetSetter() {
			return materialPropertySetter as SingleMaterialPropertySetter;
		}
	}
}
