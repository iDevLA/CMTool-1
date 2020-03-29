// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.Services
{
	using ConceptMatrix.Offsets;

	public delegate void SelectionEvent(Selection selection);

	public interface ISelectionService : IService
	{
		event SelectionEvent SelectionChanged;

		Selection CurrentSelection
		{
			get;
		}
	}

	public class Selection
	{
		public readonly Types Type;
		public readonly BaseAddresses BaseAddress;

		public Selection(Types type, BaseAddresses address)
		{
			this.Type = type;
			this.BaseAddress = address;
		}

		public enum Types
		{
			Character,
		}
	}
}
