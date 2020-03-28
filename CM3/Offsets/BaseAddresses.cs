// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.Offsets
{
	using System;

	public enum BaseAddresses
	{
		None,
		GPose,
	}

	#pragma warning disable SA1649
	public static class BaseAddressesExtensions
	{
		public static string GetOffset(this BaseAddresses address, OffsetsRoot root)
		{
			switch (address)
			{
				case BaseAddresses.None: return string.Empty;
				case BaseAddresses.GPose: return root.GposeOffset;
			}

			throw new Exception($"Unrecognized base address offset: {address}");
		}
	}
}
