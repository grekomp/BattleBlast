using UnityEngine;
using System.Collections;
using System;

public class DisposableMonoBehaviour : MonoBehaviour
{
	Coroutine disposingCoroutine;

	/// <summary>
	/// Calls HandleDispose without actually destroying the object, allowing it to be reused later.
	/// </summary>
	public virtual void Dispose(Action onDisposed = null)
	{
		disposingCoroutine = StartCoroutine(HandleDispose(onDisposed));
	}
	public virtual void DisposeAndDestroy() => DisposeAndDestroy(null);
	public virtual void DisposeAndDestroy(Action onDisposed)
	{
		if (Application.isPlaying)
		{
			disposingCoroutine = StartCoroutine(HandleDispose(() =>
			{
				Destroy();
				onDisposed?.Invoke();
			}));
		}
	}
	/// <summary>
	/// Can be used to clean up the object for reusing.
	/// </summary>
	protected virtual IEnumerator HandleDispose(Action onDisposed)
	{
		onDisposed.Invoke();
		yield return 0;
	}
	public virtual void CancelDispose()
	{
		StopCoroutine(disposingCoroutine);
	}

	public virtual void Destroy()
	{
		gameObject.DestroyAnywhere();
	}
}
