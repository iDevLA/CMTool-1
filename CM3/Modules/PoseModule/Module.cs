// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.PoseModule
{
	using System;
	using System.Threading.Tasks;
	using ConceptMatrix;
	using ConceptMatrix.Modules;
	using ConceptMatrix.Services;

	public class Module : ModuleBase
	{
		public override Task Initialize(IServices services)
		{
			Log.Write("Hello!", "Pose Module");

			IViewService viewService = services.Get<IViewService>();
			viewService.AddView<SimplePoseView>("Pose");

			return Task.CompletedTask;
		}

		public override Task Shutdown()
		{
			return Task.CompletedTask;
		}
	}
}
