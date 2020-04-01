// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
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

		private void OnTitleBarMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			this.ActiveBorder.Visibility = Visibility.Visible;
			this.InActiveBorder.Visibility = Visibility.Collapsed;
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			this.ActiveBorder.Visibility = Visibility.Collapsed;
			this.InActiveBorder.Visibility = Visibility.Visible;
		}

		private void OnCloseClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void OnMinimiseClick(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}
	}
}
