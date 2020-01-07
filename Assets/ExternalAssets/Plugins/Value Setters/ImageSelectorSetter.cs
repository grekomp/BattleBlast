using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageSelectorSetter : ValueSetter {
	[Header("Components")]
	public Image image;

	[Header("Variables")]
	[HandleChanges] public FloatReference selectorValue;
	public ImageSelectorSequence batteryStatusSetter;

	protected override void ApplySet() {
		image.sprite = batteryStatusSetter.GetSprite(selectorValue);
	}

	protected override void Init() {
		if (image == null) image = GetComponent<Image>();
	}
}
