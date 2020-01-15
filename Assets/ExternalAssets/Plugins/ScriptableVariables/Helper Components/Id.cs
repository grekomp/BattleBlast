using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

[AttributeUsage(AttributeTargets.Field)]
public class Id : Attribute
{
#if UNITY_EDITOR
	[MenuItem("Custom/CopyNewId")]
	public static void CopyNewId()
	{
		GUIUtility.systemCopyBuffer = Guid.NewGuid().ToString();
	}
#endif
}
