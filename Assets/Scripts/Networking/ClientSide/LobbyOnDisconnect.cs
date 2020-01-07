using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyOnDisconnect : MonoBehaviour
{
	[SerializeField] string sceneName;
	[SerializeField] GameEvent presentationUnloaded;
	void Awake()
	{
		ConnectionClient.instance.onDisconnectedFromServer += LoadLobby;
	}

	void LoadLobby(ServerData data)
	{
		//LoadSceneManager.instance.isPresentationLoaded = false;
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
		presentationUnloaded?.Raise();
	}
}
