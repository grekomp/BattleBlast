using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorSetter : ValueSetter {
	[Header("Components")]
	public Graphic image;

	[Header("Variables")]
	[HandleChanges] public ColorReference color;

	protected override void ApplySet() {
		if (image) image.color = color;
	}

	protected override void Init() {
		if (image == null) image = GetComponentInChildren<Graphic>();
	}
}
