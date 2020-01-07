using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ImageSelectorSequence")]
public class ImageSelectorSequence : ScriptableObject {
	[Serializable]
	public class ImageStep {
		public SpriteReference sprite;
		public FloatReference minValue;
	}

	public List<ImageStep> steps = new List<ImageStep>();
	public SpriteReference defaultSprite;

	public Sprite GetSprite(float selector) {
		Sprite selectedSprite = defaultSprite;
		foreach (var imageStep in steps) {
			if (imageStep.minValue > selector) {
				return selectedSprite;
			}
			else {
				selectedSprite = imageStep.sprite;
			}
		}

		return selectedSprite;
	}

	private void OnValidate() {
		steps.Sort((a, b) => a.minValue.Value.CompareTo(b.minValue.Value));
	}
}
