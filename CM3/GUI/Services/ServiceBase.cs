// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public abstract class ServiceBase
	{
		public abstract Task Initialize();
		public abstract Task Shutdown();
	}
}
