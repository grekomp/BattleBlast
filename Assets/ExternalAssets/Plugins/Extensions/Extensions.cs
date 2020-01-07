using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.IO;
using Object = UnityEngine.Object;
using System.Text;

public static class ExtensionMethods
{
	public static int ToInt(this bool b)
	{
		return b ? 1 : 0;
	}

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

	public static void DestroyChildren(this Transform trans)
	{
		int childCount = trans.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			GameObject.DestroyImmediate(trans.GetChild(i).gameObject);
		}
	}

	public static GameObject PlacePrefabAsChild(this Transform obj, GameObject prefab, string name, bool resetTransform)
	{
		GameObject createdObj = (GameObject)GameObject.Instantiate(prefab);
		createdObj.transform.SetParent(obj);
		if (!string.IsNullOrEmpty(name))
		{
			createdObj.transform.name = name;
		}
		if (resetTransform)
		{
			createdObj.transform.localPosition = Vector3.zero;
			createdObj.transform.localRotation = Quaternion.identity;
			createdObj.transform.localScale = Vector3.one;
		}
		return createdObj;
	}

	public static GameObject PlacePrefabAsChild(this RectTransform obj, GameObject prefab, string name, bool resetTransform)
	{
		GameObject createdObj = obj.transform.PlacePrefabAsChild(prefab, name, resetTransform);
		if (resetTransform)
		{
			RectTransform rect = createdObj.GetComponent<RectTransform>();
			if (rect != null)
			{
				rect.anchoredPosition = Vector2.zero;
				rect.sizeDelta = Vector2.zero;
			}
		}
		return createdObj;
	}

	public static T PlacePrefabAsChildAndGetComponent<T>(this Transform obj, GameObject prefab, string name, bool resetTransform)
	{
		GameObject createdObj = obj.PlacePrefabAsChild(prefab, name, resetTransform);
		return createdObj.GetComponent<T>();
	}


	public static void SetLayerRecursively(this GameObject go, string layerName)
	{
		foreach (Transform child in go.GetComponentsInChildren<Transform>(true))
		{
			child.gameObject.layer = LayerMask.NameToLayer(layerName);
		}
	}

	public static int ChildCountActive(this Transform t)
	{
		int k = 0;
		foreach (Transform c in t)
		{
			if (c.gameObject.activeSelf)
				k++;
		}
		return k;
	}

	public static int Round(this double d, int accuracy)
	{
		return Mathf.RoundToInt((float)d / accuracy) * accuracy;
	}

	public static int Round(this float f, int accuracy)
	{
		return Mathf.RoundToInt(f / accuracy) * accuracy;
	}

	public static string ToTimeString(this int seconds)
	{
		string text = "";
		int h = seconds / 3600;
		int m = (seconds - h * 3600) / 60;
		int s = (seconds - h * 3600 - m * 60);
		if (h > 0)
		{
			text += h.ToString() + "h ";
		}
		if (m > 0)
		{
			text += m.ToString() + "m ";
		}
		text += s.ToString() + "s";
		return text;
	}

	public static string ToTimeString(this float seconds)
	{
		int s = (int)seconds;
		return s.ToTimeString();
	}

	public static string RemoveOrphants(this string text)
	{
		string[] words = text.Split(' ');
		string processedText = "";
		for (int i = 0; i < words.Length - 1; i++)
		{
			processedText += words[i];
			if (words[i].Length < 5)
			{
				processedText += "\u00A0";
			}
			else
			{
				processedText += " ";
			}
		}
		processedText += words[words.Length - 1];
		return processedText;
	}

	public static string ToStandarizedPathSeparators(this string path)
	{
		path = path.Replace('\\', Path.DirectorySeparatorChar);
		path = path.Replace('/', Path.DirectorySeparatorChar);
		return path;
	}

	public static bool IsPointerOverUIObject(this EventSystem current, Vector3 pointerPosition, LayerMask? layerMask = null)
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(current);
		eventDataCurrentPosition.position = new Vector2(pointerPosition.x, pointerPosition.y);
		System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
		if (EventSystem.current != null)
		{
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		}
		if (layerMask != null)
		{
			LayerMask mask = layerMask.GetValueOrDefault();
			return results.Find(x => mask == (mask | (1 << x.gameObject.layer))).isValid;
		}
		else
		{
			return results.Count > 0;
		}
	}

	public static bool IsPointerOverUIObject(this EventSystem current)
	{
		return current.IsPointerOverUIObject(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
	}

	public static Color HexToColor(this string hex)
	{
		if (string.IsNullOrEmpty(hex))
		{
			return Color.white;
		}
		else
		{
			hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
			hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
			byte a = 255;//assume fully visible unless specified in hex
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			//Only use alpha if the string has enough characters
			if (hex.Length == 8)
			{
				a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			}
			return new Color32(r, g, b, a);
		}
	}

	public static Coroutine InvokeWithDelay(this MonoBehaviour mb, System.Action action, float delay)
	{
		return mb.StartCoroutine(InvokeWithSecDelay(action, delay));
	}

	public static Coroutine InvokeAtNextFrame(this MonoBehaviour mb, System.Action action)
	{
		return mb.StartCoroutine(InvokeWithFrameDelay(action));
	}

	static System.Collections.IEnumerator InvokeWithSecDelay(System.Action action, float delay)
	{
		WaitForSeconds secondsDelay = new WaitForSeconds(delay);
		yield return secondsDelay;
		if (action != null)
		{
			action.Invoke();
		}
	}

	static System.Collections.IEnumerator InvokeWithFrameDelay(System.Action action)
	{
		yield return null;
		if (action != null)
		{
			action.Invoke();
		}
	}

	public static Camera GetMainCamera()
	{
		Camera targetCamera = null;
		Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
		if (cameras.Length > 0)
		{
			if (cameras.Length > 1)
			{
				targetCamera = Camera.main;
			}
			if (targetCamera == null)
			{
				targetCamera = cameras[0];
			}
		}
		return targetCamera;
	}

	public static bool SceneExists(string sceneName)
	{
		List<string> scenePaths = new List<string>();
		for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; ++i)
		{
			scenePaths.Add(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
		}
		return (scenePaths.Find(x => x.Contains(sceneName)) != null);
	}

	public static string[] ToSimpleArray(this string[][] array)
	{
		List<string> simpleArray = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			string[] innerArray = array[i];
			for (int j = 0; j < innerArray.Length; j++)
			{
				simpleArray.Add(innerArray[j]);
			}
		}
		return simpleArray.ToArray();
	}

	public static float Cross(this Vector2 original, Vector2 other)
	{
		return original.x * other.y - original.y * other.x;
	}

	public static Vector2? Intersection(this RectTransform rect, Vector2 origin, Vector2 offset)
	{
		//left side of the rect
		Vector2 rectSideToCheckOrigin = rect.anchoredPosition + new Vector2(rect.rect.x, rect.rect.y);
		Vector2 rectSideToCheckOffset = new Vector2(0, rect.rect.height);
		Vector2? temp = ExtensionMethods.Intersection(origin, offset, rectSideToCheckOrigin, rectSideToCheckOffset);
		if (temp == null)
		{
			//bottom side of the rect
			rectSideToCheckOrigin = rect.anchoredPosition + new Vector2(rect.rect.x, rect.rect.y);
			rectSideToCheckOffset = new Vector2(rect.rect.width, 0);
			temp = ExtensionMethods.Intersection(origin, offset, rectSideToCheckOrigin, rectSideToCheckOffset);
			if (temp == null)
			{
				//right side of the rect
				rectSideToCheckOrigin = rect.anchoredPosition + new Vector2(rect.rect.x + rect.rect.width, rect.rect.y);
				rectSideToCheckOffset = new Vector2(0, rect.rect.height);
				temp = ExtensionMethods.Intersection(origin, offset, rectSideToCheckOrigin, rectSideToCheckOffset);
				if (temp == null)
				{
					//top side of the rect
					rectSideToCheckOrigin = rect.anchoredPosition + new Vector2(rect.rect.x, rect.rect.y + rect.rect.height);
					rectSideToCheckOffset = new Vector2(rect.rect.width, 0);
					temp = ExtensionMethods.Intersection(origin, offset, rectSideToCheckOrigin, rectSideToCheckOffset);
				}
			}
		}
		return temp;
	}

	public static Vector2? Intersection(Vector2 v1origin, Vector2 v1offset, Vector2 v2origin, Vector2 v2offset)
	{
		float crossOfOffsets = v1offset.Cross(v2offset);
		float crossOfOffsetBetweenOriginsAndV1Offset = (v2origin - v1origin).Cross(v1offset);
		if (crossOfOffsets == 0)
		{
			if (crossOfOffsetBetweenOriginsAndV1Offset == 0)
			{
				//collinear
				float t0 = Vector2.Dot((v2origin - v1origin), v1offset) / Vector2.Dot(v1offset, v1offset);
				float t1 = t0 + Vector2.Dot(v2offset, v1offset) / Vector2.Dot(v1offset, v1offset);
				if ((t0 < 0 && t1 < 0) || (t0 > 1 && t1 > 1))
				{
					//collinear but does not cover one another
					return null;
				}
				else
				{
					return v1origin + v1offset * t0;
				}
			}
			else
			{
				//parallel
				return null;
			}
		}
		else
		{
			float t = (v2origin - v1origin).Cross(v2offset) / (crossOfOffsets);
			float u = (v2origin - v1origin).Cross(v1offset) / (crossOfOffsets);
			if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
			{
				return v1origin + t * v1offset;
			}
			else
			{
				//not parallel but don't intersect
				return null;
			}
		}
	}

	///UNTESTED!
	public static bool LineTriangleIntersection(List<Vector3> triangle, Vector3 A, Vector3 B, out Vector3 intersection)
	{
		Vector3 normal;

		normal = Vector3.Cross(triangle[1] - triangle[0], triangle[2] - triangle[0]);
		normal.Normalize();
		Debug.Log(normal);

		Vector3 intersectionPoint = (triangle[0] + triangle[1] + triangle[2]) / 3f;
		Debug.DrawLine(intersectionPoint, intersectionPoint + normal * 1000, Color.red, Mathf.Infinity);
		GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = intersectionPoint;

		float dist1 = Vector3.Dot((A - triangle[0]), normal);
		float dist2 = Vector3.Dot((B - triangle[0]), normal);

		intersection = A + (B - A) * (-dist1 / (dist2 - dist1));
		if (dist1 == dist2 || (dist1 * dist2) >= 0)
		{
			return false;
		}

		Vector3 test = Vector3.Cross(normal, (triangle[1] - triangle[0]));
		if (Vector3.Dot(test, intersection - triangle[0]) < 0)
		{
			return false;
		}
		test = Vector3.Cross(normal, (triangle[2] - triangle[1]));
		if (Vector3.Dot(test, intersection - triangle[1]) < 0)
		{
			return false;
		}
		test = Vector3.Cross(normal, (triangle[0] - triangle[2]));
		if (Vector3.Dot(test, intersection - triangle[0]) < 0)
		{
			return false;
		}
		return true;
	}

	public static void CopyComponent(this Component original, GameObject target)
	{
		System.Type type = original.GetType();
		Component copy = target.AddComponent(type);
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
	}

	public static void Hide(this CanvasGroup canvasGroup)
	{
		canvasGroup.alpha = 0;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
	}

	public static void Show(this CanvasGroup canvasGroup)
	{
		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;
	}

	public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
	{
		if (rectTransform == null) return;

		Vector2 size = rectTransform.rect.size;
		Vector2 deltaPivot = rectTransform.pivot - pivot;
		Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
		rectTransform.pivot = pivot;
		rectTransform.localPosition -= deltaPosition;
	}

	public static void SetAnchorsWithoutChangingPosition(this RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax)
	{
		Vector3 tempPositon = rectTransform.position;
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;
		rectTransform.position = tempPositon;
	}

	public static string FirstCharToUpper(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		return char.ToUpper(input[0]) + input.Substring(1);
	}

	public static string FirstCharToLower(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		return char.ToLower(input[0]) + input.Substring(1);
	}

	public static float AspectRatio(this Sprite sprite)
	{
		return sprite.rect.width / sprite.rect.height;
	}

	public static void SafeInvoke(this Action action)
	{
		if (action != null) action();
	}

	public static void SafeInvoke<T>(this Action<T> action, T arg)
	{
		if (action != null) action(arg);
	}

	public static float CompletionPercentage(this AudioSource source)
	{
		return source.time / source.clip.length;
	}

	public static bool CaseInsensitiveContains(this string text, string value,
	StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
	{
		return text.IndexOf(value, stringComparison) >= 0;
	}
	public static T Next<T>(this T src) where T : struct
	{
		if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

		T[] Arr = (T[])Enum.GetValues(src.GetType());
		int j = Array.IndexOf<T>(Arr, src) + 1;
		return (Arr.Length == j) ? Arr[0] : Arr[j];
	}

	public static IEnumerable<TSource> DistinctBy<TSource, TKey>
	(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		HashSet<TKey> seenKeys = new HashSet<TKey>();
		foreach (TSource element in source)
		{
			if (seenKeys.Add(keySelector(element)))
			{
				yield return element;
			}
		}
	}

	public static T GetOrAdd<T>(this GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		if (component == null)
		{
			component = go.AddComponent<T>();
		}
		return component;
	}

#if UNITY_EDITOR
	public static Type GetFieldType(this UnityEditor.SerializedProperty serializedProperty)
	{
		Type parentType = serializedProperty.serializedObject.targetObject.GetType();
		return parentType.GetFieldViaPath(serializedProperty.propertyPath).FieldType;
	}
#endif

	public static System.Reflection.FieldInfo GetFieldViaPath(this Type type, string path)
	{
		Type parentType = type;
		System.Reflection.FieldInfo fi = null;
		string[] perDot = path.Split('.');
		foreach (string fieldName in perDot)
		{
			fi = parentType.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			if (fi == null)
			{
				fi = parentType.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			}

			if (fi != null)
				parentType = fi.FieldType;
			else
				return null;
		}
		if (fi != null)
			return fi;
		else return null;
	}

	public static string ToLatinUppercase(this int value)
	{
		StringBuilder result = new StringBuilder();
		while (value >= 0)
		{
			result.Insert(0, (char)('A' + value % 26));
			value /= 26;
			value--;
		}
		return result.ToString();
	}
	public static string ToLatinLowercase(this int value)
	{
		StringBuilder result = new StringBuilder();
		while (value >= 0)
		{
			result.Insert(0, (char)('a' + value % 26));
			value /= 26;
			value--;
		}
		return result.ToString();
	}

	public static string ToRoman(this int number)
	{
		StringBuilder result = new StringBuilder();

		number++;
		while (number > 0)
		{
			if (number >= 1000)
			{
				result.Append("M");
				number -= 1000;
				continue;
			}
			if (number >= 900)
			{
				result.Append("CM");
				number -= 900;
				continue;
			}
			if (number >= 500)
			{
				result.Append("D");
				number -= 500;
				continue;
			}
			if (number >= 400)
			{
				result.Append("CD");
				number -= 400;
				continue;
			}
			if (number >= 100)
			{
				result.Append("C");
				number -= 100;
				continue;
			}
			if (number >= 90)
			{
				result.Append("XC");
				number -= 90;
				continue;
			}
			if (number >= 50)
			{
				result.Append("L");
				number -= 50;
				continue;
			}
			if (number >= 40)
			{
				result.Append("XL");
				number -= 40;
				continue;
			}
			if (number >= 10)
			{
				result.Append("X");
				number -= 10;
				continue;
			}
			if (number >= 9)
			{
				result.Append("IX");
				number -= 9;
				continue;
			}
			if (number >= 5)
			{
				result.Append("V");
				number -= 5;
				continue;
			}
			if (number >= 4)
			{
				result.Append("IV");
				number -= 4;
				continue;
			}
			if (number >= 1)
			{
				result.Append("I");
				number -= 1;
				continue;
			}
		}

		return result.ToString();
	}
}