using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Camera")]
public class CameraVariable : ScriptableVariable<Camera> {
    public static CameraVariable New(Camera value = default) {
        var createdVariable = CreateInstance<CameraVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
