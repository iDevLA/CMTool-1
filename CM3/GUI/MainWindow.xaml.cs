// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Text;
	using System.Windows;
	using ConceptMatrix;

	/// <summary>
	/// Interaction logic for MainWindow.xaml.
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			Log.OnLog += this.OnLog;
		}

		private void OnLog(string message, string category)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				this.LogDisplay.Content = $"[{category}] {message}";
			});
		}
	}
}
