using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItSilesiaPlugins {
	public abstract class MaterialPropertySetter {
		public class WeightedSingleSetter {
			public SingleMaterialPropertySetter setter;
			public float weight;

			public WeightedSingleSetter(SingleMaterialPropertySetter setter, float weight) {
				this.setter = setter;
				this.weight = weight;
			}
		}

		public abstract void Set(Material material);
		public abstract void InitFrom(Material material);

		public static MaterialPropertySetterGroup GetBlendedSetter(List<MaterialPropertySetter> setters, List<float> weights, List<SingleMaterialPropertySetter> defaults = null) {
			if (setters.Count != weights.Count) {
				Debug.LogError("MaterialPropertySetter: GetBlendedSetter: The number of setters and weights passed are not equal.");

				while (setters.Count > weights.Count) {
					weights.Add(1f);
				}
			}

			List<WeightedSingleSetter> singleSetters = GetWeightedSingleSetters(setters, weights);
			return GetBlendedSetter(singleSetters, defaults);
		}
		public static MaterialPropertySetterGroup GetBlendedSetter(List<WeightedSingleSetter> singleSetters, List<SingleMaterialPropertySetter> defaults = null) {
			MaterialPropertySetterGroup blendedSetter = new MaterialPropertySetterGroup();
			List<WeightedSingleSetter> remainingSetters = new List<WeightedSingleSetter>(singleSetters);

			foreach (var singleSetter in singleSetters) {
				if (remainingSetters.Contains(singleSetter) == false) continue;

				List<WeightedSingleSetter> samePropertySetters = GetSamePropertySetters(remainingSetters, singleSetter.setter.propertyName);

				float weightSum = samePropertySetters.Select(s => s.weight).Sum();
				if (defaults != null && weightSum < 1) {
					var defaultPropertySetter = defaults.Find(s => s.propertyName == singleSetter.setter.propertyName);
					samePropertySetters.Add(new WeightedSingleSetter(defaultPropertySetter, 1f - weightSum));
				}

				BlendSetters(blendedSetter, samePropertySetters);

				remainingSetters = remainingSetters.Except(samePropertySetters).ToList();
			}

			return blendedSetter;
		}


		#region Helper methods
		private static void BlendSetters(MaterialPropertySetterGroup blendedSetter, List<WeightedSingleSetter> samePropertySetters) {
			switch (samePropertySetters[0].setter) {
				case FloatMaterialPropertySetter floatMaterialPropertySetter: {
						blendedSetter.floatSetters.Add(BlendFloatSetters(samePropertySetters));
					}
					break;
				case IntMaterialPropertySetter intMaterialPropertySetter: {
						blendedSetter.intSettters.Add(BlendIntSetters(samePropertySetters));
					}
					break;
				case TextureMaterialPropertySetter textureMaterialPropertySetter: {
						blendedSetter.textureSetters.Add(BlendTextureSetters(samePropertySetters));
					}
					break;
				case VectorMaterialPropertySetter vectorMaterialPropertySetter: {
						blendedSetter.vectorSetters.Add(BlendVectorSetters(samePropertySetters));
					}
					break;
				case ColorMaterialPropertySetter colorMaterialPropertySetter: {
						blendedSetter.colorSetters.Add(BlendColorSetters(samePropertySetters));
					}
					break;
				default:
					Debug.LogWarning("MaterialPropertySetter: GetBlendedSetter: Warning: Blending setters of type " + samePropertySetters[0].setter.GetType() + " is not implemented");
					break;
			}
		}
		private static FloatMaterialPropertySetter BlendFloatSetters(List<WeightedSingleSetter> samePropertySetters) {
			float summaricWeight = 0f;
			float summaricValue = 0f;
			List<WeightedSingleSetter> sameTypeSetters = GetSettersOfType<FloatMaterialPropertySetter>(samePropertySetters);

			foreach (var setter in sameTypeSetters) {
				FloatMaterialPropertySetter floatSetter = setter.setter as FloatMaterialPropertySetter;
				summaricValue += floatSetter.value * setter.weight;
				summaricWeight += setter.weight;
			}

			var blendedValue = summaricWeight != 0 ? (summaricValue / summaricWeight) : 0;
			return new FloatMaterialPropertySetter() {
				propertyName = samePropertySetters[0].setter.propertyName,
				value = blendedValue,
			};
		}
		private static IntMaterialPropertySetter BlendIntSetters(List<WeightedSingleSetter> samePropertySetters) {
			float summaricWeight = 0;
			float summaricValue = 0;
			List<WeightedSingleSetter> sameTypeSetters = GetSettersOfType<IntMaterialPropertySetter>(samePropertySetters);

			foreach (var setter in sameTypeSetters) {
				IntMaterialPropertySetter intSetter = setter.setter as IntMaterialPropertySetter;
				summaricValue += intSetter.value * setter.weight;
				summaricWeight += setter.weight;
			}

			var blendedValue = summaricWeight != 0 ? (int)(summaricValue / summaricWeight) : 0;
			return new IntMaterialPropertySetter() {
				propertyName = samePropertySetters[0].setter.propertyName,
				value = blendedValue,
			};
		}
		private static TextureMaterialPropertySetter BlendTextureSetters(List<WeightedSingleSetter> samePropertySetters) {
			Texture maxWeightTexture = null;
			float maxWeight = 0f;

			List<WeightedSingleSetter> sameTypeSetters = GetSettersOfType<TextureMaterialPropertySetter>(samePropertySetters);

			foreach (var setter in sameTypeSetters) {
				if (setter.weight > maxWeight) {
					maxWeightTexture = ((TextureMaterialPropertySetter)setter.setter).value;
					maxWeight = setter.weight;
				}
			}

			return new TextureMaterialPropertySetter() {
				propertyName = samePropertySetters[0].setter.propertyName,
				value = maxWeightTexture,
			};
		}
		private static VectorMaterialPropertySetter BlendVectorSetters(List<WeightedSingleSetter> samePropertySetters) {
			float summaricWeight = 0;
			Vector3 summaricValue = Vector3.zero;
			List<WeightedSingleSetter> sameTypeSetters = GetSettersOfType<VectorMaterialPropertySetter>(samePropertySetters);

			foreach (var setter in sameTypeSetters) {
				VectorMaterialPropertySetter vectorSetter = setter.setter as VectorMaterialPropertySetter;
				summaricValue += vectorSetter.value * setter.weight;
				summaricWeight += setter.weight;
			}

			var blendedValue = summaricWeight != 0 ? summaricValue / summaricWeight : Vector3.zero;
			return new VectorMaterialPropertySetter() {
				propertyName = samePropertySetters[0].setter.propertyName,
				value = blendedValue,
			};
		}
		private static ColorMaterialPropertySetter BlendColorSetters(List<WeightedSingleSetter> samePropertySetters) {
			float summaricWeight = 0;
			Color summaricValue = Color.black;
			List<WeightedSingleSetter> sameTypeSetters = GetSettersOfType<ColorMaterialPropertySetter>(samePropertySetters);

			foreach (var setter in sameTypeSetters) {
				ColorMaterialPropertySetter typedSetter = setter.setter as ColorMaterialPropertySetter;
				summaricValue += typedSetter.value * setter.weight;
				summaricWeight += setter.weight;
			}

			var blendedValue = summaricWeight != 0 ? summaricValue / summaricWeight : Color.black;

			return new ColorMaterialPropertySetter() {
				propertyName = samePropertySetters[0].setter.propertyName,
				value = blendedValue,
			};
		}

		/// <summary>
		/// Generates a list of all SingleMaterialPropertySetters with corresponding weights
		/// </summary>
		public static List<WeightedSingleSetter> GetWeightedSingleSetters(List<MaterialPropertySetter> setters, List<float> weights) {
			List<WeightedSingleSetter> singleSetters = new List<WeightedSingleSetter>();

			int i = 0;
			foreach (var setter in setters) {
				if (setter is MaterialPropertySetterGroup setterGroup) {
					foreach (var subSetter in setterGroup.GetSubSetters()) {
						singleSetters.Add(new WeightedSingleSetter(subSetter, weights[i]));
					}
				}

				if (setter is SingleMaterialPropertySetter singleSetter) {
					singleSetters.Add(new WeightedSingleSetter(singleSetter, weights[i]));
				}

				i++;
			}

			return singleSetters;
		}
		private static List<WeightedSingleSetter> GetSamePropertySetters(List<WeightedSingleSetter> weightedSingleSetters, string propertyName) {
			List<WeightedSingleSetter> result = new List<WeightedSingleSetter>();

			foreach (var setter in weightedSingleSetters) {
				if (setter.setter.propertyName == propertyName) {
					result.Add(setter);
				}
			}

			return result;
		}
		private static List<WeightedSingleSetter> GetSettersOfType<T>(List<WeightedSingleSetter> samePropertySetters) where T : SingleMaterialPropertySetter {
			return samePropertySetters.Where((ws) => ws.setter is T).ToList();
		}
		#endregion
	}
}
