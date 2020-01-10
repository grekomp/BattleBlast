using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskExtensions
{
	public static IEnumerator AsIEnumerator(this Task task)
	{
		while (task.IsCompleted == false && task.IsCanceled == false)
		{
			yield return null;
		}

		if (task.IsCanceled)
		{
			throw new TimeoutException("The tested task has timed out, and was cancelled");
		}

		if (task.IsFaulted || task.Exception != null)
		{
			throw task.Exception;
		}

		yield return null;
	}

	public static IEnumerator RunTaskAsIEnumerator(Func<Task> function, int timeoutMs = -1)
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		if (timeoutMs > 0)
		{
			cancellationTokenSource.CancelAfter(timeoutMs);
		}
		return RunTaskAsIEnumerator(function, cancellationTokenSource.Token);
	}
	public static IEnumerator RunTaskAsIEnumerator(Func<Task> function, CancellationToken ct)
	{
		return AsIEnumerator(Task.Run(function, ct));
	}

	internal static CancellationToken GetTimeoutCancellationToken(int timeoutMs)
	{
		CancellationTokenSource cts = new CancellationTokenSource();
		cts.CancelAfter(timeoutMs);
		return cts.Token;
	}
}
