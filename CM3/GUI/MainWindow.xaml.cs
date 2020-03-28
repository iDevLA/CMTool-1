// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Windows;
	using System.Windows.Controls;
	using ConceptMatrix;
	using ConceptMatrix.GUI.Services;

	/// <summary>
	/// Interaction logic for MainWindow.xaml.
	/// </summary>
	public partial class MainWindow : Window
	{
		private Dictionary<string, Type> views = new Dictionary<string, Type>();
		private UserControl currentView;

		public MainWindow()
		{
			this.InitializeComponent();

			ModuleService.AddView += this.OnAddView;
			Log.OnLog += this.OnLog;
		}

		private void OnAddView(string path, Type view)
		{
			if (this.views.ContainsKey(path))
				throw new Exception($"View already registered at path: {path}");

			if (!typeof(UserControl).IsAssignableFrom(view))
				throw new Exception($"View: {view} does not extend from UserControl.");

			this.views.Add(path, view);

			Application.Current?.Dispatcher.Invoke(() =>
			{
				this.ViewList.Items.Add(path);
			});
		}

		private void OnLog(string message, string category)
		{
			if (Application.Current == null)
				return;

			Application.Current.Dispatcher.Invoke(() =>
			{
				this.LogDisplay.Content = $"[{category}] {message}";
			});
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string path = (string)this.ViewList.SelectedItem;
			Type viewType = this.views[path];

			this.currentView = (UserControl)Activator.CreateInstance(viewType);
			this.ViewArea.Content = this.currentView;
		}
	}
}
