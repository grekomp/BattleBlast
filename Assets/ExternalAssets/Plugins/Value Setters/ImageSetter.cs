using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSetter : ValueSetter {

	[Header("Components")]
	public Image image;

	[Header("Variables")]
	[HandleChanges] public SpriteReference sprite;

	protected override void ApplySet() {
		if (image) image.sprite = sprite;
	}

	protected override void Init() {
		if (image == null) image = GetComponent<Image>();
	}
}
