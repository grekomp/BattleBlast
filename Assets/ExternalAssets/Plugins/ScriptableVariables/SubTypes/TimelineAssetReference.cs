using System;
using UnityEngine;
using UnityEngine.Timeline;

[Serializable]
public class TimelineAssetReference : ScriptableVariableReference<TimelineAsset, TimelineAssetVariable> {
	public TimelineAssetReference() : base() { }
	public TimelineAssetReference(TimelineAsset value) : base(value) { }
	public TimelineAssetReference(TimelineAssetVariable variable) : base(variable) { }
}
