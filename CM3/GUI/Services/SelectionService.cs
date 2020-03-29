// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI.Services
{
	using System.Threading.Tasks;
	using ConceptMatrix;
	using ConceptMatrix.Offsets;
	using ConceptMatrix.Services;

	public class SelectionService : ISelectionService
	{
		private Selection currentSelection;

		public event SelectionEvent SelectionChanged;

		public bool IsAlive
		{
			get;
			private set;
		}

		public Selection CurrentSelection
		{
			get
			{
				return this.currentSelection;
			}
		}

		public Task Initialize(IServices services)
		{
			this.IsAlive = true;
			return Task.CompletedTask;
		}

		public Task Shutdown()
		{
			this.IsAlive = false;
			return Task.CompletedTask;
		}

		public Task Start()
		{
			Task.Run(this.Watch);
			return Task.CompletedTask;
		}

		private async Task Watch()
		{
			IInjectionService injection = App.Services.Get<IInjectionService>();

			string oldActorId = null;
			while (this.IsAlive)
			{
				// TODO: I suspect Jhoto knows a better way to get the active selection.
				IMemory<string> actorId = injection.GetMemory<string>(BaseAddresses.GPose, injection.Offsets.Character.ActorID);

				string newActorId = actorId.Get();

				if (newActorId != oldActorId)
				{
					this.currentSelection = new Selection(Selection.Types.Character, BaseAddresses.GPose);
					this.SelectionChanged?.Invoke(this.currentSelection);

					oldActorId = newActorId;
				}

				await Task.Delay(100);
			}
		}
	}
}
