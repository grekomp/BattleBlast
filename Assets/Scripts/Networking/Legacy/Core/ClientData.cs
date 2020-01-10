using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS0618 // Type or member is obsolete
[System.Serializable]
public class ClientData
{
	public string deviceId { get => _deviceId; private set => _deviceId = value; }
	public int connectionId { get => _connectionId; private set => _connectionId = value; }
	public int clientId { get => _clientId; private set => _clientId = value; }
	public string serialNumber { get => _serialNumber; private set => _serialNumber = value; }
	public string ip { get => _ip; private set => _ip = value; }

	[SerializeField]
	private string _deviceId;
	[SerializeField]
	private int _connectionId;
	[SerializeField]
	private int _clientId;
	[SerializeField]
	private string _serialNumber;
	[SerializeField]
	private string _ip;

	public ClientData(int connectionId, string ip)
	{
		this.connectionId = connectionId;
		clientId = connectionId;
		this.ip = ip.Substring(7, ip.Length - 7);
	}

	public void IntroduceMe(IntroduceResponseData data)
	{
		deviceId = data.deviceId;
		serialNumber = data.serialNumber;
	}

	public override string ToString()
	{
		return string.Format("Client {0} connected on {1}", deviceId, connectionId);
	}

	public int GetPing()
	{
		byte error;
		int ping = NetworkTransport.GetCurrentRTT(ConnectionServer.instance.connectionHostId, clientId, out error);

		if ((NetworkError)error != NetworkError.Ok)
		{
			DebugNet.Log((NetworkError)error);
		}

		return ping;
	}
}
#pragma warning restore CS0618 // Type or member is obsolete
