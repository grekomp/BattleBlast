using System;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
#pragma warning disable CS0618 // Type or member is obsolete
public class ConnectionConfigReference : ScriptableVariableReference<ConnectionConfig, ConnectionConfigVariable>
{
	public ConnectionConfigReference() : base() { }
	public ConnectionConfigReference(ConnectionConfig value) : base(value) { }
	public ConnectionConfigReference(ConnectionConfigVariable variable) : base(variable) { }
}
#pragma warning restore CS0618 // Type or member is obsolete
