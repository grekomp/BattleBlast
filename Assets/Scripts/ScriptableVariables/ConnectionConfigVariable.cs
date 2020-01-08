using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "Scriptable Variables/ConnectionConfig")]
#pragma warning disable CS0618 // Type or member is obsolete
public class ConnectionConfigVariable : ScriptableVariable<ConnectionConfig>
{
	public static ConnectionConfigVariable New(ConnectionConfig value = default)
	{
		var createdVariable = CreateInstance<ConnectionConfigVariable>();
		createdVariable.Value = value;
		return createdVariable;
	}
}
#pragma warning restore CS0618 // Type or member is obsolete
