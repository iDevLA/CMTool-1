// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.PoseModule
{
	using System;
	using System.Threading.Tasks;
	using ConceptMatrix;
	using ConceptMatrix.Modules;

	public class PoseModule : IModule
	{
		public Task Initialize()
		{
			Log.Write("Hello!", "Pose Module");
			return Task.CompletedTask;
		}

		public Task Shutdown()
		{
			return Task.CompletedTask;
		}
	}
}
