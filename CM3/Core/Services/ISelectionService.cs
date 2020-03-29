// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.Services
{
	using ConceptMatrix.Offsets;

	public delegate void SelectionEvent(SelectionArgs args);

	public interface ISelectionService : IService
	{
		event SelectionEvent SelectionChanged;
	}

	public class SelectionArgs
	{
		public Types Type;
		public BaseAddresses BaseAddress;

		public enum Types
		{
			Character,
		}
	}
}
