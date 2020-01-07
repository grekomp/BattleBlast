using System;
using UnityEngine;
[Serializable]
public class CameraReference : ScriptableVariableReference<Camera, CameraVariable> {
    public CameraReference() : base() { }
    public CameraReference(Camera value) : base(value) { }
    public CameraReference(CameraVariable variable) : base(variable) { }
}
