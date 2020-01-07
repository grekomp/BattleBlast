using UnityEngine;
using System.Collections;
using TMPro;

public class TextColorStyleSetter : ValueSetter {
	[Header("Components")]
	public TextMeshProUGUI text;
	[Header("Variables")]
	[HandleChanges] public ColorReference color;
	[HandleChanges] public FontStylesReference style;

	protected override void ApplySet() {
		if (text == null) return;

		if (text.color != color.Value)
			text.color = color.Value;
		if (text.fontStyle != style)
			text.fontStyle = style;
	}

	protected override void Init() {
		if (text == null) text = GetComponent<TextMeshProUGUI>();
	}
}
