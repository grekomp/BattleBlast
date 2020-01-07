using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSetter : ValueSetter {
	[Header("Components")]
	public TextMeshProUGUI text;
	[Header("Variables")]
	[HandleChanges] public StringReference value;

	protected override void ApplySet() {
		if (text && text.text != value) {
			text.text = value;
		}
	}

	protected override void Init() {
		if (text == null) text = GetComponent<TextMeshProUGUI>();
	}
}
