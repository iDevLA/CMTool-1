// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Threading.Tasks;
	using ConceptMatrix.Modules;

	using static ConceptMatrix.Modules.ModuleBase;

	public class ModuleService : ServiceBase
	{
		private List<ModuleBase> modules = new List<ModuleBase>();

		public static event ViewEvent AddView;

		public override Task Initialize()
		{
			return this.InitializeModules("./Modules/");
		}

		public override Task Shutdown()
		{
			return this.ShutdownModules();
		}

		private async Task InitializeModules(string directory)
		{
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			DirectoryInfo directoryInfo = new DirectoryInfo(directory);
			FileInfo[] assemblies = directoryInfo.GetFiles("*.dll", SearchOption.AllDirectories);

			foreach (FileInfo assemblyInfo in assemblies)
			{
				Assembly assembly = Assembly.LoadFrom(assemblyInfo.FullName);
				await this.InitializeModules(assembly);
			}
		}

		private async Task InitializeModules(Assembly targetAssembly)
		{
			try
			{
				foreach (Type type in targetAssembly.GetTypes())
				{
					if (type.IsAbstract || type.IsInterface)
						continue;

					if (typeof(ModuleBase).IsAssignableFrom(type))
					{
						ModuleBase module = (ModuleBase)Activator.CreateInstance(type);
						module.AddModuleView += this.OnAddModuleView;
						await module.Initialize();
					}
				}
			}
			catch (Exception ex)
			{
				Log.Write(new Exception($"Failed to initialize module assembly: {targetAssembly.FullName}", ex));
			}
		}

		private async Task ShutdownModules()
		{
			foreach (ModuleBase module in this.modules)
			{
				module.AddModuleView -= this.OnAddModuleView;
				await module.Shutdown();
			}
		}

		private void OnAddModuleView(string path, Type view)
		{
			AddView?.Invoke(path, view);
		}
	}
}
