using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererColorSetter : ValueSetter {
    [Header("Components")]
    public LineRenderer line;
    [Header("Variables")]
    [HandleChanges] public ColorReference value;

    protected override void ApplySet() {
        if (line) line.sharedMaterial.color = value;
    }

    protected override void Init() {
        if (line == null) line = GetComponent<LineRenderer>();
    }
}
