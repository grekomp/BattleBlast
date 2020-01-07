using UnityEngine;

public class ExitApplication : MonoBehaviour
{
	public void Exit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
