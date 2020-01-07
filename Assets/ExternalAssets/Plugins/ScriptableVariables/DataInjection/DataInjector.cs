using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

public class DataInjector : DataInjectionReceiver
{
	[Header("Data")]
	public List<DataInjectorDataEntry> dataEntries = new List<DataInjectorDataEntry>();

	[Header("Events")]
	public List<DataInjectorGameEventEntry> gameEventEntries = new List<DataInjectorGameEventEntry>();

	[Header("Injected DataBundle")]
	public DataBundleReference injectedDataBundle;

	#region DataInjection
	public void Inject(DataBundle dataBundle)
	{
		injectedDataBundle.Value = dataBundle;
		ReinjectCurrentDataBundle();
	}
	private void ReinjectCurrentDataBundle()
	{
		if (injectedDataBundle.Value == null) return;

		foreach (var dataPair in injectedDataBundle.Value.data)
		{
			if (dataPair != null) Inject(dataPair.key, dataPair.data);
		}

		// Replace all variables that were not injected with an empty variable
		foreach (var dataEntry in dataEntries)
		{
			if (dataEntry == null) continue;

			if (dataEntry.replacementVariable == null && dataEntry.variableToReplace != null && dataEntry.variableToReplace.name.StartsWith("_r"))
			{
				ReplaceVariable(dataEntry, dataEntry.variableToReplace.CreateCopy());
			}
		}

		foreach (var eventPair in injectedDataBundle.Value.events)
		{
			if (eventPair != null) Inject(eventPair.key, eventPair.gameEvent);
		}
	}

	public void Inject(string key, ScriptableVariable variable)
	{
		DataInjectorDataEntry injectableData = FindDataEntry(key);
		if (injectableData != null)
		{
			ReplaceVariable(injectableData, variable);
		}
	}
	public void Inject(ScriptableVariable from, ScriptableVariable to)
	{
		DataInjectorDataEntry injectableData = FindDataEntry(from);
		if (injectableData != null)
		{
			ReplaceVariable(injectableData, to);
		}
	}

	public bool ReplaceVariable(DataInjectorDataEntry entry, ScriptableVariable to)
	{
		bool replacementSuccessful;

		if (entry.replacementVariable == null)
		{
			replacementSuccessful = ReplaceVariable(entry.variableToReplace, to);
		}
		else
		{
			replacementSuccessful = ReplaceVariable(entry.replacementVariable, to);
		}

		entry.replacementVariable = to;
		return replacementSuccessful;
	}
	public override bool ReplaceVariable(ScriptableVariable from, ScriptableVariable to)
	{
		bool replacementSuccessful = false;
		foreach (var dataReceiver in GetInjectionReceivers(gameObject))
		{
			bool variableReplaced = dataReceiver.ReplaceVariable(from, to);
			replacementSuccessful = replacementSuccessful || variableReplaced;
		}

		return replacementSuccessful;
	}
	#endregion

	#region GameEventInjection
	public void Inject(string key, GameEvent gameEvent)
	{
		var entry = FindGameEventEntry(key);
		if (entry != null)
		{
			ReplaceEvent(entry, gameEvent);
		}
	}
	public void Inject(GameEvent from, GameEvent to)
	{
		var entry = FindGameEventEntry(from);
		if (entry != null)
		{
			ReplaceEvent(entry, to);
		}
	}

	public bool ReplaceEvent(DataInjectorGameEventEntry entry, GameEvent to)
	{
		bool replacementSuccessful;

		if (entry.replacementEvent == null)
		{
			replacementSuccessful = ReplaceEvent(entry.eventToReplace, to);
		}
		else
		{
			replacementSuccessful = ReplaceEvent(entry.replacementEvent, to);
		}

		entry.replacementEvent = to;
		return replacementSuccessful;
	}
	public override bool ReplaceEvent(GameEvent from, GameEvent to)
	{
		bool replacementSuccessful = false;
		foreach (var dataReceiver in GetInjectionReceivers(gameObject))
		{
			bool eventReplaced = dataReceiver.ReplaceEvent(from, to);
			replacementSuccessful = replacementSuccessful || eventReplaced;
		}

		return replacementSuccessful;
	}
	#endregion

	#region Helpers
	protected List<DataInjectionReceiver> GetInjectionReceivers(GameObject obj)
	{
		List<DataInjectionReceiver> dataInjectionReceivers = new List<DataInjectionReceiver>();
		dataInjectionReceivers.AddRange(obj.GetComponents<DataInjectionReceiver>());

		foreach (Transform child in obj.transform)
		{
			DataInjector injector = child.GetComponent<DataInjector>();
			if (injector)
			{
				dataInjectionReceivers.Add(injector);
			}
			else
			{
				dataInjectionReceivers.AddRange(GetInjectionReceivers(child.gameObject));
			}
		}

		dataInjectionReceivers.Remove(this);
		return dataInjectionReceivers;
	}

	protected List<ScriptableVariable> GetAllInjectableVariablesInChildren()
	{
		List<ScriptableVariable> foundVariables = new List<ScriptableVariable>();

		foreach (DataInjectionReceiver injectionReceiver in GetInjectionReceivers(gameObject))
		{
			foundVariables.AddRange(injectionReceiver.GetInjectableVariables());
			foundVariables = foundVariables.Distinct().ToList();
		}

		return foundVariables;
	}
	protected List<GameEvent> GetAllInjectableEventsInChildren()
	{
		List<GameEvent> injectableEvents = new List<GameEvent>();

		foreach (DataInjectionReceiver injectionReceiver in GetInjectionReceivers(gameObject))
		{
			injectableEvents.AddRange(injectionReceiver.GetInjectableEvents());
			injectableEvents = injectableEvents.Distinct().ToList();
		}

		return injectableEvents;
	}

	protected DataInjectorDataEntry FindDataEntry(string key)
	{
		return dataEntries.Find((d) => d.key == key);
	}
	protected DataInjectorDataEntry FindDataEntry(ScriptableVariable variable)
	{
		return dataEntries.Find((d) => d.variableToReplace == variable);
	}

	protected DataInjectorGameEventEntry FindGameEventEntry(string key)
	{
		return gameEventEntries.Find(e => e.key == key);
	}
	protected DataInjectorGameEventEntry FindGameEventEntry(GameEvent gameEvent)
	{
		return gameEventEntries.Find(e => e.eventToReplace == gameEvent);
	}

	[ContextMenu("GenerateKeys")]
	protected void GenerateKeysByVariableName()
	{
		EditorUtilityExtensions.RecordObject(this, "Generate keys");

		foreach (var dataEntry in dataEntries)
		{
			if (string.IsNullOrEmpty(dataEntry.key.Value) && dataEntry.variableToReplace != null)
			{
				dataEntry.key.Value = GenerateKey(dataEntry.variableToReplace.name);
			}
		}

		foreach (var eventEntry in gameEventEntries)
		{
			if (string.IsNullOrEmpty(eventEntry.key.Value) && eventEntry.eventToReplace != null)
			{
				eventEntry.key.Value = GenerateKey(eventEntry.eventToReplace.name);
			}
		}

		EditorUtilityExtensions.RecordPrefabInstancePropertyModifications(this);
	}
	protected string GenerateKey(string name)
	{
		string[] splitName = name.Split(' ');

		StringBuilder sb = new StringBuilder();

		if (splitName[0] != "_r")
		{
			sb.Append(splitName[0]);
			if (splitName.Count() > 1)
			{
				sb.Append("_");
			}
		}

		for (int i = 1; i < splitName.Count(); i++)
		{
			if (i > 1) sb.Append("_");

			sb.Append(splitName[i]);
		}

		return sb.ToString();
	}

	[ContextMenu("GenerateEntries")]
	protected void GenerateEntries()
	{
		foreach (var injectableVariable in GetAllInjectableVariablesInChildren())
		{
			if (dataEntries.Find(e => e.variableToReplace == injectableVariable) == null)
			{
				dataEntries.Add(new DataInjectorDataEntry() { variableToReplace = injectableVariable });
			}
		}

		foreach (var injectableEvent in GetAllInjectableEventsInChildren())
		{
			if (gameEventEntries.Find(e => e.eventToReplace == injectableEvent) == null)
			{
				gameEventEntries.Add(new DataInjectorGameEventEntry() { eventToReplace = injectableEvent });
			}
		}

		GenerateKeysByVariableName();
	}
	[ContextMenu("GenerateEntriesWithPrefix")]
	protected void GenerateEntriesWithPrefix()
	{
		foreach (var injectableVariable in GetAllInjectableVariablesInChildren())
		{
			if (injectableVariable.name.StartsWith("_r") && dataEntries.Find(e => e.variableToReplace == injectableVariable) == null)
			{
				dataEntries.Add(new DataInjectorDataEntry() { variableToReplace = injectableVariable });
			}
		}

		foreach (var injectableEvent in GetAllInjectableEventsInChildren())
		{
			if (injectableEvent.name.StartsWith("_r") && gameEventEntries.Find(e => e.eventToReplace == injectableEvent) == null)
			{
				gameEventEntries.Add(new DataInjectorGameEventEntry() { eventToReplace = injectableEvent });
			}
		}

		GenerateKeysByVariableName();
	}

	public override List<ScriptableVariable> GetInjectableVariables()
	{
		List<ScriptableVariable> injectableVariables = GetAllInjectableVariablesInChildren();
		injectableVariables.AddRange(dataEntries.Where(e => e.variableToReplace != null).Select(e => e.variableToReplace).ToList());
		return injectableVariables.Distinct().ToList();
	}
	public override List<GameEvent> GetInjectableEvents()
	{
		List<GameEvent> injectableEvents = GetAllInjectableEventsInChildren();
		injectableEvents.AddRange(gameEventEntries.Where(e => e.eventToReplace != null).Select(e => e.eventToReplace).ToList());
		return injectableEvents.Distinct().ToList();
	}
	#endregion
}
