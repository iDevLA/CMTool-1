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

		public abstract Task Initialize();
		public abstract Task Shutdown();

		public void AddView<T>(string path)
		{
			this.AddModuleView?.Invoke(path, typeof(T));
		}
	}
}
