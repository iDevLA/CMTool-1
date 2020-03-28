// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.PoseModule
{
	using System;
	using System.Threading.Tasks;
	using ConceptMatrix;
	using ConceptMatrix.Modules;

	public class Module : ModuleBase
	{
		public override Task Initialize(IServices services)
		{
			Log.Write("Hello!", "Pose Module");

			this.AddView<PoseView>("Pose");

			return Task.CompletedTask;
		}

		public override Task Shutdown()
		{
			return Task.CompletedTask;
		}
	}
}
