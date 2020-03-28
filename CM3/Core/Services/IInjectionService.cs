// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.Services
{
	using ConceptMatrix.Offsets;

	public interface IInjectionService : IService
	{
		OffsetsRoot Offsets { get; }
		IMemory<T> GetMemory<T>(params string[] offsets);
		string GetBaseAddress(string address);
	}
}
