// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Threading.Tasks;
	using System.Windows;
	using ConceptMatrix;

	/// <summary>
	/// Interaction logic for App.xaml.
	/// </summary>
	public partial class App : Application
	{
		private static ServiceManager serviceManager = new ServiceManager();

		public App()
		{
			this.Exit += this.OnAppExit;
			Log.OnError += this.OnError;

			this.MainWindow = new ConceptMatrix.GUI.MainWindow();
			this.MainWindow.Show();

			Task.Run(serviceManager.InitializeServices);
		}

		public static ServiceManager Services
		{
			get
			{
				return serviceManager;
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
			Task t = serviceManager.ShutdownServices();
			t.Wait();
		}
	}
}
