using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class DataInjectionReceiver : DisposableMonoBehaviour {
	[NonSerialized]
	ScriptableVariableReference[] injectableVariableReferences;
	[NonSerialized]
	GameEventHandler[] injectableGameEventHandlers;
	[NonSerialized]
	GameEventHandlerGroup[] injectableGameEventHandlerGroups;

	public virtual bool ReplaceVariable(ScriptableVariable from, ScriptableVariable to) {
		FindInjectableDataIfNotInitialized();

		bool replacementSuccessful = false;
		foreach (ScriptableVariableReference reference in injectableVariableReferences) {
			bool variableReplaced = reference.TryReplaceVariable(from, to);
			replacementSuccessful = replacementSuccessful || variableReplaced;
		}

		return replacementSuccessful;
	}
	public virtual bool ReplaceEvent(GameEvent from, GameEvent to) {

		bool replacementSuccessful = false;
		foreach (var handler in injectableGameEventHandlers) {
			bool eventReplaced = handler.ReplaceEvent(from, to);
			replacementSuccessful = replacementSuccessful || eventReplaced;
		}

		foreach (var handler in injectableGameEventHandlerGroups) {
			bool eventReplaced = handler.ReplaceEvent(from, to);
			replacementSuccessful = replacementSuccessful || eventReplaced;
		}

		return replacementSuccessful;
	}

	private void FindInjectableDataIfNotInitialized() {
		if (injectableVariableReferences == null) {
			FindInjectableData();
		}
	}
	private void FindInjectableData() {
		FieldInfo[] fieldInfos = this.GetType().GetFields();

		List<ScriptableVariableReference> foundVariableReferences = new List<ScriptableVariableReference>();
		List<GameEventHandler> foundGameEventHandlers = new List<GameEventHandler>();
		List<GameEventHandlerGroup> foundGameEventHandlerGroups = new List<GameEventHandlerGroup>();
		foreach (FieldInfo fieldInfo in fieldInfos) {
			if (typeof(ScriptableVariableReference).IsAssignableFrom(fieldInfo.FieldType)) {
				foundVariableReferences.Add(fieldInfo.GetValue(this) as ScriptableVariableReference);
				continue;
			}

			if (typeof(GameEventHandler).IsAssignableFrom(fieldInfo.FieldType)) {
				foundGameEventHandlers.Add(fieldInfo.GetValue(this) as GameEventHandler);
				continue;
			}

			if (typeof(GameEventHandlerGroup).IsAssignableFrom(fieldInfo.FieldType)) {
				foundGameEventHandlerGroups.Add(fieldInfo.GetValue(this) as GameEventHandlerGroup);
				continue;
			}
		}

		injectableVariableReferences = foundVariableReferences.ToArray();
		injectableGameEventHandlers = foundGameEventHandlers.ToArray();
		injectableGameEventHandlerGroups = foundGameEventHandlerGroups.ToArray();
	}

	public virtual List<ScriptableVariable> GetInjectableVariables() {
		FindInjectableDataIfNotInitialized();

		List<ScriptableVariable> injectableVariables = new List<ScriptableVariable>();
		foreach (var injectableVariableReference in injectableVariableReferences) {
			injectableVariableReference.GatherReplaceableVariables(ref injectableVariables);
		}

		return injectableVariables;
	}
	public virtual List<GameEvent> GetInjectableEvents() {
		FindInjectableDataIfNotInitialized();

		List<GameEvent> injectableEvents = new List<GameEvent>();

		foreach (GameEventHandler handler in injectableGameEventHandlers) {
			injectableEvents.Add(handler.BoundEvent);
		}
		foreach (GameEventHandlerGroup handlerGroup in injectableGameEventHandlerGroups) {
			injectableEvents.AddRange(handlerGroup.BoundEvents);
		}

		return injectableEvents.Distinct().Where(e => e != null).ToList();
	}
}
