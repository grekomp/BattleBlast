using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ItSilesiaPlugins {
	[Serializable]
	public class MaterialPropertySetterGroup : MaterialPropertySetter {
		public List<MaterialPropertySetterPreset> setterPresets = new List<MaterialPropertySetterPreset>();

		public List<FloatMaterialPropertySetter> floatSetters = new List<FloatMaterialPropertySetter>();
		public List<IntMaterialPropertySetter> intSettters = new List<IntMaterialPropertySetter>();
		public List<TextureMaterialPropertySetter> textureSetters = new List<TextureMaterialPropertySetter>();
		public List<VectorMaterialPropertySetter> vectorSetters = new List<VectorMaterialPropertySetter>();
		public List<ColorMaterialPropertySetter> colorSetters = new List<ColorMaterialPropertySetter>();

		public void Add(SingleMaterialPropertySetter singleMaterialPropertySetter) {
			if (singleMaterialPropertySetter is FloatMaterialPropertySetter floatSetter) floatSetters.Add(floatSetter);
			if (singleMaterialPropertySetter is IntMaterialPropertySetter intSetter) intSettters.Add(intSetter);
			if (singleMaterialPropertySetter is TextureMaterialPropertySetter textureSetter) textureSetters.Add(textureSetter);
			if (singleMaterialPropertySetter is VectorMaterialPropertySetter vectorSetter) vectorSetters.Add(vectorSetter);
			if (singleMaterialPropertySetter is ColorMaterialPropertySetter colorSetter) colorSetters.Add(colorSetter);
		}
		public override void Set(Material material) {
			foreach (var setter in setterPresets) {
				setter?.Set(material);
			}

			foreach (var setter in floatSetters) {
				setter?.Set(material);
			}

			foreach (var setter in intSettters) {
				setter?.Set(material);
			}

			foreach (var setter in textureSetters) {
				setter?.Set(material);
			}

			foreach (var setter in vectorSetters) {
				setter?.Set(material);
			}

			foreach (var setter in colorSetters) {
				setter?.Set(material);
			}
		}

		public List<SingleMaterialPropertySetter> GetSubSetters() {
			List<SingleMaterialPropertySetter> subSetters = new List<SingleMaterialPropertySetter>();

			foreach (var setter in setterPresets) {
				subSetters.Add(setter.GetSetter());
			}

			foreach (var setter in floatSetters) {
				subSetters.Add(setter);
			}

			foreach (var setter in intSettters) {
				subSetters.Add(setter);
			}

			foreach (var setter in textureSetters) {
				subSetters.Add(setter);
			}

			foreach (var setter in vectorSetters) {
				subSetters.Add(setter);
			}

			foreach (var setter in colorSetters) {
				subSetters.Add(setter);
			}

			return subSetters;
		}

		public override void InitFrom(Material material) {
			foreach (var subSetter in GetSubSetters()) {
				subSetter.InitFrom(material);
			}
		}
	}
}
