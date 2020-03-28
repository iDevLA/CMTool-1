// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix
{
	using System;

	public static class Log
	{
		public delegate void LogEvent(string message, string category);
		public delegate void ErrorEvent(Exception ex, string category);

		public static event LogEvent OnLog;
		public static event ErrorEvent OnError;

		public static void Write(string message, string category = null)
		{
			Console.WriteLine($"[{category}] {message}");
			OnLog?.Invoke(message, category);
		}

		public static void Write(Exception ex, string category = null)
		{
			Console.WriteLine($"[{category}] {ex.Message}");
			OnError?.Invoke(ex, category);
		}
	}
}
