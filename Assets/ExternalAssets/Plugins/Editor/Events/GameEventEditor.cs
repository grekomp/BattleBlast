using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		GameEvent gameEvent = target as GameEvent;

		DrawInvocationsList(gameEvent.GetOnEventRaisedInvocationsList(), "Registered Listeners");
		DrawInvocationsList(gameEvent.GetOnEventDataRaisedInvocationsList(), "Registered Listeners With Data");

		if (GUILayout.Button("Raise")) {
			gameEvent.Raise();
		}
	}

	private static void DrawInvocationsList(Delegate[] invocationsList, string header) {
		if (invocationsList != null && invocationsList.Count() > 0) {
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(header, EditorStyles.boldLabel);

			foreach (var item in invocationsList) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.ObjectField(item.Target as UnityEngine.Object, item.Target.GetType(), true);
				EditorGUILayout.LabelField(item.Method.ToString());
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();
		}
	}
}
