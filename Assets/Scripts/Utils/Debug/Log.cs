using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
	public enum LogLevel
	{
		Debug,      // Temporary development-only logs
		Error,      // Logs about bad and impossible to ignore situations
		Warning,    // Logs about potentially unwanted situations, that were handled by the system
		Info,       // Informational logs about general / important events and states of the system
		Verbose,    // Detailed informational logs that might be helpful to inspect the inner workings of a system
		WTF         // What a Terrible Failure = Critical situations that should never happen
	}

	public static class Log
	{
		public readonly static string defaultDebugTag = "QuickDebug";

		public static void PrintLog(LogLevel logLevel, string tag, object message, UnityEngine.Object context = null)
		{
			string logString = $"{logLevel}: {tag}: {message}";

			if (context == null)
			{
				switch (logLevel)
				{
					case LogLevel.Debug:
					case LogLevel.Info:
					case LogLevel.Verbose:
						Debug.Log(logString);
						break;
					case LogLevel.Error:
					case LogLevel.WTF:
						Debug.LogError(logString);
						break;
					case LogLevel.Warning:
						Debug.LogWarning(logString);
						break;
				}
			}
			else
			{
				switch (logLevel)
				{
					case LogLevel.Debug:
					case LogLevel.Info:
					case LogLevel.Verbose:
						Debug.Log(logString, context);
						break;
					case LogLevel.Error:
					case LogLevel.WTF:
						Debug.LogError(logString, context);
						break;
					case LogLevel.Warning:
						Debug.LogWarning(logString, context);
						break;
				}
			}
		}

		public static void D(string message)
		{
			PrintLog(LogLevel.Debug, defaultDebugTag, message);
		}
		public static void D(string tag, object message, UnityEngine.Object context = null)
		{
			PrintLog(LogLevel.Debug, tag, message, context);
		}
		public static void Error(string tag, object message, UnityEngine.Object context = null)
		{
			PrintLog(LogLevel.Error, tag, message, context);
		}
		public static void Warning(string tag, object message, UnityEngine.Object context = null)
		{
			PrintLog(LogLevel.Warning, tag, message, context);
		}
		public static void Info(string tag, object message, UnityEngine.Object context = null)
		{
			PrintLog(LogLevel.Info, tag, message, context);
		}
		public static void Verbose(string tag, object message, UnityEngine.Object context = null)
		{
			PrintLog(LogLevel.Verbose, tag, message, context);
		}
		public static void WTF(string tag, object message, UnityEngine.Object context = null)
		{
			PrintLog(LogLevel.WTF, tag, message, context);
		}
	}
}
