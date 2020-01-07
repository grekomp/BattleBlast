using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RawImageSetter : ValueSetter {
	[Header("Components")]
	public RawImage image;

	[Header("Variables")]
	[HandleChanges] public TextureReference texture;

	protected override void ApplySet() {
		if (image) image.texture = texture;
	}

	protected override void Init() {
		if (image == null) image = GetComponentInChildren<RawImage>();
	}
}
