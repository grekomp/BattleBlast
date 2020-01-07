using UnityEngine;

public class SkinnedMeshRendererColorSetter : ValueSetter {
    [Header("Components")]
    public SkinnedMeshRenderer mesh;
    [Header("Variables")]
    [HandleChanges] public ColorReference value;

    protected override void ApplySet() {
        if (mesh) mesh.sharedMaterial.color = value;
    }

    protected override void Init() {
        if (mesh == null) mesh = GetComponent<SkinnedMeshRenderer>();
    }
}
