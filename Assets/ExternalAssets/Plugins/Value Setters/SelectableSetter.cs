using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableSetter : ValueSetter {
	[Header("Components")]
	public Selectable target;

	[Header("Variables")]
	[HandleChanges] public ColorReference normalColor;
	[HandleChanges] public ColorReference highlightedColor;
	[HandleChanges] public ColorReference pressedColor;
	[HandleChanges] public ColorReference selectedColor;
	[HandleChanges] public ColorReference disabledColor;
	[HandleChanges] public FloatReference colorMultiplier = new FloatReference(1);
	[Space]
	[HandleChanges] public BoolReference interactable = new BoolReference(true);

	protected override void ApplySet() {
		if (target == null) return;

		ColorBlock colorBlock = new ColorBlock();
		colorBlock.colorMultiplier = colorMultiplier;
		colorBlock.normalColor = normalColor;
		colorBlock.highlightedColor = highlightedColor;
		colorBlock.pressedColor = pressedColor;
		colorBlock.selectedColor = selectedColor;
		colorBlock.disabledColor = disabledColor;

		target.colors = colorBlock;

		target.interactable = interactable;
	}

	protected override void Init() {
		if (target == null) target = GetComponentInChildren<Selectable>();
	}
}
