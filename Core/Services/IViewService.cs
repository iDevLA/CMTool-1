// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.Services
{
	public interface IViewService : IService
	{
		/// <summary>
		/// Adds a view of type T to the navigation menu at path.
		/// </summary>
		/// <typeparam name="T">the type of view to add. Should extend UserControl.</typeparam>
		/// <param name="path">the path to add the menu item in the navigation sidebar.</param>
		// Although we could (where T : UserControl) to require correct types, doing so would
		// require the core library to reference the WPF libs, and for simplicity, lets not.
		void AddView<T>(string path);
	}
}
