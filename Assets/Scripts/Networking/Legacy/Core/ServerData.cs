using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerData {

	public int connectionId;
	public string serverIp;
	public string serverMessage;

	public ServerData(int connectionId, string serverIp, string serverMessage) {
		this.connectionId = connectionId;
		this.serverIp = serverIp;
		this.serverMessage = serverMessage;
	}
}
