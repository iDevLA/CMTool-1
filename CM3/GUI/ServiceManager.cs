// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using ConceptMatrix.GUI.Services;
	using ConceptMatrix.Services;

	public class ServiceManager : IServices
	{
		private List<IService> services = new List<IService>();
		private bool isStarted = false;

		public T Get<T>()
			where T : IService
		{
			throw new NotImplementedException();
		}

		public async Task AddService<T>()
			where T : IService, new()
		{
			try
			{
				Log.Write($"Adding service: {typeof(T).Name}", "Services");
				IService service = Activator.CreateInstance<T>();
				this.services.Add(service);
				await service.Initialize(this);

				// If we've already started, and this service is being added late (possibly by a module from its start method) start the service immediately.
				if (this.isStarted)
				{
					await service.Start();
				}
			}
			catch (Exception ex)
			{
				Log.Write(new Exception($"Failed to initialize service: {typeof(T).Name}", ex));
			}
		}

		public async Task InitializeServices()
		{
			await this.AddService<InjectionService>();
			await this.AddService<ModuleService>();

			Log.Write($"Services Initialized", "Services");
		}

		public async Task StartServices()
		{
			// Since starting a service _can_ add new services, copy the list first.
			List<IService> services = new List<IService>(this.services);
			foreach (IService service in services)
			{
				await service.Start();
			}

			this.isStarted = true;
			Log.Write($"Services Started", "Services");
		}

		public async Task ShutdownServices()
		{
			foreach (IService service in this.services)
			{
				await service.Shutdown();
			}
		}
	}
}
