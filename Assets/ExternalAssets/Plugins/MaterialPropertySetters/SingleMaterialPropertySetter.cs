using UnityEngine;
using System.Collections;
using System;

namespace ItSilesiaPlugins {
	public abstract class SingleMaterialPropertySetter : MaterialPropertySetter {
		public string propertyName;

		public abstract SingleMaterialPropertySetter Copy();
	}

	[Serializable]
	public abstract class SingleMaterialPropertySetter<T> : SingleMaterialPropertySetter {
		public T value;
	}
}