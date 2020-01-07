using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureSetter : ValueSetter {
    [Header("Components")]
    public RawImage image;
    [Header("Variables")]
    [HandleChanges] public RenderTextureReference value;

    protected override void ApplySet() {
        if(image && image.texture != value) {
            image.texture = value;
        }
    }

    protected override void Init() {
        if(image == null) image = GetComponent<RawImage>();
    }
}
