using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ImageArrayColorSetter : ValueSetter {
	[Header("Variables")]
	public ColorReference activeColor;
	public ColorReference inactiveColor;

	[HandleChanges] public IntReference fillLevel;

	List<Image> images = new List<Image>();

	protected override void ApplySet() {
		int clampedDifficulty = Mathf.Clamp(fillLevel, 1, images.Count);

		int i = 0;
		foreach (Image image in images) {
			if (clampedDifficulty > i) {
				image.color = activeColor;
			}
			else {
				image.color = inactiveColor;
			}
			i++;
		}
	}

	protected override void Init() {
		images = GetComponentsInChildren<Image>().ToList();
	}
}
