// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Controls;
	using ConceptMatrix;
	using ConceptMatrix.GUI.Services;

	/// <summary>
	/// Interaction logic for MainWindow.xaml.
	/// </summary>
	public partial class MainWindow : Window
	{
		private UserControl currentView;
		private ViewService viewService;

		public MainWindow()
		{
			this.InitializeComponent();

			this.viewService = App.Services.Get<ViewService>();

			this.viewService.AddingView += this.OnAddView;

			foreach (string path in this.viewService.ViewPaths)
			{
				this.OnAddView(path);
			}
		}

		private void OnAddView(string path)
		{
			Application.Current?.Dispatcher.Invoke(() =>
			{
				this.ViewList.Items.Add(path);
			});
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string path = (string)this.ViewList.SelectedItem;
			Type viewType = this.viewService.GetView(path);

			try
			{
				this.currentView = (UserControl)Activator.CreateInstance(viewType);
				this.ViewArea.Content = this.currentView;
			}
			catch (TargetInvocationException ex)
			{
				Log.Write(new Exception($"Failed to create view: {viewType}", ex.InnerException));
			}
			catch (Exception ex)
			{
				Log.Write(new Exception($"Failed to create view: {viewType}", ex));
			}
		}
	}
}
