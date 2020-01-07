using UnityEngine;

public class MeshRendererColorSetter : ValueSetter {
	[Header("Components")]
	public MeshRenderer mesh;
	[Header("Variables")]
	[HandleChanges] public ColorReference value;

	protected override void ApplySet() {
		if (mesh) mesh.sharedMaterial.color = value;
	}

	protected override void Init() {
		if (mesh == null) mesh = GetComponent<MeshRenderer>();
	}
}
