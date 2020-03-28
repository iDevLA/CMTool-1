// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI.Services
{
	using System.Threading.Tasks;
	using ConceptMatrix.Offsets;

	public class InjectionService : ServiceBase
	{
		private Root offsets;

		public override Task Initialize()
		{
			this.offsets = OffsetsManager.LoadSettings(OffsetsManager.Regions.Live);

			return Task.CompletedTask;
		}

		public override Task Shutdown()
		{
			return Task.CompletedTask;
		}
	}
}
