// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Windows;
	using ConceptMatrix;
	using ConceptMatrix.GUI.Services;

	/// <summary>
	/// Interaction logic for App.xaml.
	/// </summary>
	public partial class App : Application
	{
		private static List<ServiceBase> services = new List<ServiceBase>();

		public App()
		{
			this.Exit += this.OnAppExit;
			Log.OnError += this.OnError;

			this.MainWindow = new ConceptMatrix.GUI.MainWindow();
			this.MainWindow.Show();

			Task.Run(this.InitializeServices);
		}

		public static async Task AddService<T>()
			where T : ServiceBase, new()
		{
			try
			{
				Log.Write($"Adding service: {typeof(T).Name}", "Application");
				ServiceBase service = Activator.CreateInstance<T>();
				services.Add(service);
				await service.Initialize();
			}
			catch (Exception ex)
			{
				Log.Write(new Exception($"Failed to initialize service: {typeof(T).Name}", ex));
			}
		}

		private void OnError(Exception ex, string category)
		{
			if (Application.Current == null)
				return;

			Application.Current.Dispatcher.Invoke(() =>
			{
				ErrorDialog dlg = new ErrorDialog(ex);
				dlg.Owner = this.MainWindow;
				dlg.ShowDialog();
			});
		}

		private void OnAppExit(object sender, ExitEventArgs e)
		{
			Task t = this.ShutdownServices();
			t.Wait();
		}

		private async Task InitializeServices()
		{
			await App.AddService<InjectionService>();
			await App.AddService<ModuleService>();

			Log.Write($"Services Initialized", "Application");
		}

		private async Task ShutdownServices()
		{
			foreach (ServiceBase service in services)
			{
				await service.Shutdown();
			}
		}
	}
}
