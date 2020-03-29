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
		public event SelectionEvent SelectionChanged;

		public bool IsAlive
		{
			get;
			private set;
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
					SelectionArgs args = new SelectionArgs();
					args.Type = SelectionArgs.Types.Character;
					args.BaseAddress = BaseAddresses.GPose;
					this.SelectionChanged?.Invoke(args);

					oldActorId = newActorId;
				}

				await Task.Delay(100);
			}
		}
	}
}
