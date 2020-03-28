// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.Modules
{
	using System;
	using System.Threading.Tasks;

	public abstract class ModuleBase
	{
		public delegate void ViewEvent(string path, Type view);

		public event ViewEvent AddModuleView;

		public static IServices Services { get; private set; }

		/// <summary>
		/// Called when the module is loaded into memory during service initialization.
		/// </summary>
		public virtual Task Initialize(IServices services)
		{
			Services = services;
			return Task.CompletedTask;
		}

		/// <summary>
		/// Called once all services are ready.
		/// </summary>
		public virtual Task Start()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Called as the application quits.
		/// </summary>
		public virtual Task Shutdown()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Add a custom view to the sidebar at the given path.
		/// </summary>
		public void AddView<T>(string path)
		{
			this.AddModuleView?.Invoke(path, typeof(T));
		}
	}
}
