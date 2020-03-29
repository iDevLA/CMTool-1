// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System.Windows;

	/// <summary>
	/// Interaction logic for SplashWindow.xaml.
	/// </summary>
	public partial class SplashWindow : Window
	{
		public SplashWindow()
		{
			this.InitializeComponent();

			ServiceManager.OnServiceInitializing += this.ServiceManager_OnServiceInitializing;
			ServiceManager.OnServiceStarting += this.ServiceManager_OnServiceStarting;
		}

		private void ServiceManager_OnServiceStarting(string serviceName)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				this.ServiceName.Content = "Starting " + serviceName;
			});
		}

		private void ServiceManager_OnServiceInitializing(string serviceName)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				this.ServiceName.Content = "Initializing " + serviceName;
			});
		}
	}
}
