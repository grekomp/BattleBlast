using UnityEngine;
using System.Collections;

public class IntervalGameEventSender : MonoBehaviour {
	public GameEventHandlerGroup gameEvents;
	public FloatReference interval;

	Coroutine runningCoroutine;

	private void OnEnable() {
		runningCoroutine = StartCoroutine(RaiseEverySeconds());
	}
	private void OnDisable() {
		StopCoroutine(runningCoroutine);
	}

	IEnumerator RaiseEverySeconds() {
		while (true) {
			gameEvents.Raise();
			yield return new WaitForSeconds(interval);
		}
	}
}
