// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.Modules
{
	using System.Threading.Tasks;

	public interface IModule
	{
		Task Initialize();
		Task Shutdown();
	}
}
