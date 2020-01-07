using System;
using UnityEngine;
[Serializable]
public class GameObjectReference : ScriptableVariableReference<GameObject, GameObjectVariable> {
    public GameObjectReference() : base() { }
    public GameObjectReference(GameObject value) : base(value) { }
    public GameObjectReference(GameObjectVariable variable) : base(variable) { }
}
