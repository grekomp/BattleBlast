using System;
using TMPro;
using UnityEngine;
[Serializable]
public class FontStylesReference : ScriptableVariableReference<FontStyles, FontStylesVariable> {
	public FontStylesReference() : base() { }
	public FontStylesReference(FontStyles value) : base(value) { }
	public FontStylesReference(FontStylesVariable variable) : base(variable) { }
}
