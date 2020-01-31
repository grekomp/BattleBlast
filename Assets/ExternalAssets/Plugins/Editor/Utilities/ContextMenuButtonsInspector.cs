using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
[CanEditMultipleObjects]
public class ContextMenuButtonsInspector : Editor {
	protected static bool FORCE_INIT = false;
	[DidReloadScripts]
	private static void HandleScriptReload() {
		FORCE_INIT = true;

		EditorApplication.delayCall = () => { EditorApplication.delayCall = () => { FORCE_INIT = false; }; };
	}

	protected bool isInitialized = false;
	protected bool hasEditable = false;

	protected struct ContextMenuData {
		public string menuItem;
		public MethodInfo function;
		public MethodInfo validate;

		public ContextMenuData(string item) {
			menuItem = item;
			function = null;
			validate = null;
		}
	}

	protected Dictionary<string, ContextMenuData> contextData = new Dictionary<string, ContextMenuData>();

	~ContextMenuButtonsInspector() {
		isInitialized = false;
	}

	#region Initialization
	private void OnEnable() {
		InitInspector();
	}

	protected virtual void InitInspector(bool force) {
		if (force)
			isInitialized = false;
		InitInspector();
	}

	protected virtual void InitInspector() {
		if (isInitialized && FORCE_INIT == false)
			return;

		FindContextMenu();
	}

	private IEnumerable<MethodInfo> GetAllMethods(Type t) {
		if (t == null)
			return Enumerable.Empty<MethodInfo>();
		var binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		return t.GetMethods(binding).Concat(GetAllMethods(t.BaseType));
	}

	private void FindContextMenu() {
		contextData.Clear();

		// Get context menu
		Type targetType = target.GetType();
		Type contextMenuType = typeof(ContextMenu);
		MethodInfo[] methods = GetAllMethods(targetType).ToArray();
		for (int index = 0; index < methods.GetLength(0); ++index) {
			MethodInfo methodInfo = methods[index];
			foreach (ContextMenu contextMenu in methodInfo.GetCustomAttributes(contextMenuType, false)) {
				if (contextData.ContainsKey(contextMenu.menuItem)) {
					var data = contextData[contextMenu.menuItem];
					if (contextMenu.validate)
						data.validate = methodInfo;
					else
						data.function = methodInfo;
					contextData[data.menuItem] = data;
				}
				else {
					var data = new ContextMenuData(contextMenu.menuItem);
					if (contextMenu.validate)
						data.validate = methodInfo;
					else
						data.function = methodInfo;
					contextData.Add(data.menuItem, data);
				}
			}
		}
	}
	#endregion

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		DrawContextMenuButtons();
	}

	protected enum IterControl {
		Draw,
		Continue,
		Break
	}

	#region Helper functions
	public void DrawContextMenuButtons() {
		if (contextData.Count == 0) return;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Context Menu", EditorStyles.boldLabel);
		foreach (KeyValuePair<string, ContextMenuData> kv in contextData) {
			bool enabledState = GUI.enabled;
			bool isEnabled = true;
			if (kv.Value.validate != null)
				isEnabled = (bool)kv.Value.validate.Invoke(target, null);

			GUI.enabled = isEnabled;
			if (GUILayout.Button(kv.Key) && kv.Value.function != null) {
				kv.Value.function.Invoke(target, null);
			}
			GUI.enabled = enabledState;
		}
	}
	#endregion
}
