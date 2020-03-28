// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.Injection
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Text;

	public class ProcessInjection
	{
		private Process process;

		private ProcessModule mainModule;
		private Dictionary<string, IntPtr> modules = new Dictionary<string, IntPtr>();
		private bool is64Bit;

		public IntPtr Handle
		{
			get;
			private set;
		}

		/// <summary>
		/// Open the PC game process with all security and access rights.
		/// </summary>
		public void OpenProcess(int pid)
		{
			if (pid <= 0)
				throw new Exception($"Invalid process id: {pid}");

			this.process = Process.GetProcessById(pid);

			if (this.process == null)
				throw new Exception($"Failed to get process: {pid}");

			if (!this.process.Responding)
				throw new Exception("Target process id not responding");

			this.Handle = OpenProcess(0x001F0FFF, true, pid);
			Process.EnterDebugMode();

			if (this.Handle == IntPtr.Zero)
			{
				int eCode = Marshal.GetLastWin32Error();
			}

			// Set main module
			this.mainModule = this.process.MainModule;

			// Set all modules
			this.modules.Clear();
			foreach (ProcessModule module in this.process.Modules)
			{
				if (string.IsNullOrEmpty(module.ModuleName))
					continue;

				if (this.modules.ContainsKey(module.ModuleName))
					continue;

				this.modules.Add(module.ModuleName, module.BaseAddress);
			}

			this.is64Bit = Environment.Is64BitOperatingSystem && (IsWow64Process(this.Handle, out bool retVal) && !retVal);

			Debug.WriteLine($"Attached to process: {pid}");
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

		[DllImport("kernel32")]
		private static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);
	}
}
