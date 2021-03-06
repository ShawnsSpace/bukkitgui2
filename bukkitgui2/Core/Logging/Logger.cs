﻿using System;
using System.Diagnostics;

namespace Bukkitgui2.Core.Logging
{
	public static class Logger
	{
		/// <summary>
		///     Indicates wether this component is initialized and can be used
		/// </summary>
		public static bool IsInitialized { get; private set; }

		/// <summary>
		///     Create or open needed files, create streams if needed, do everything what's needed before a Log() call can be made
		/// </summary>
		internal static void Initialize()
		{
			IsInitialized = true;
		}

		/// <summary>
		///     Stop the logger, dispose used sources
		/// </summary>
		internal static void Dispose()
		{
		}

		/// <summary>
		///     Log a message to the logger
		/// </summary>
		/// <param name="level">Indicates how severe the message is. </param>
		/// <param name="origin">The class that called the log() command</param>
		/// <param name="message">The message to log</param>
		/// <param name="details">optional details, for example the message of an exception</param>
		public static void Log(LogLevel level, string origin, string message, string details = "")
		{
			//Always log to console, there are no dependencies for this
			string debugLine = TimeStamp() + " " + FormatLevel(level) + " " + origin + " : " + message + " (" + details + ")" +
			                   ";";
			Debug.WriteLine(debugLine);

			//if initialized, also log to file
			if (IsInitialized)
			{
			}
		}

		/// <summary>
		///     Save the log file to a given location
		/// </summary>
		/// <param name="savelocation">The location to save the log file. If empty default location will be used</param>
		internal static void SaveFile(string savelocation)
		{
		}

		private static string TimeStamp()
		{
			return "[" + DateTime.Now.ToLongTimeString() + "]";
		}

		private static string FormatLevel(LogLevel level)
		{
			return "[" + level + "]";
		}
	}

	/// <summary>
	///     Indicates how severe the message is.
	///     Debug is only shown as debug output.
	///     Info is meant for updates on what the program is doing
	///     Warning are handled errors where the GUI uses an alternative method to work around the issue
	///     Severe are handled errors where the GUI can't continue. The user is likely to see an error dialog and the operation
	///     is probably to be aborted.
	///     Critical is only used for unhandled errors and crashes.
	/// </summary>
	public enum LogLevel
	{
		Debug,
		Info,
		Warning,
		Severe,
		Critical
	};
}