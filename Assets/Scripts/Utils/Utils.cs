using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
	public static class Utils
	{
		public static void DestroyAnywhere(Object objectToDestroy)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == true)
			{
				Object.Destroy(objectToDestroy);
			}
			else
			{
				Object.DestroyImmediate(objectToDestroy);
			}
#else
		Object.Destroy(objectToDestroy);
#endif
		}
		public static void DestroyAnywhere(this GameObject gameObject)
		{
			DestroyAnywhere(gameObject as Object);
		}
	}
}
