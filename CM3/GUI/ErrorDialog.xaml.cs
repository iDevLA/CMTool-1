﻿// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Text;
	using System.Windows;

	/// <summary>
	/// Interaction logic for ErrorDialog.xaml.
	/// </summary>
	public partial class ErrorDialog : Window
	{
		public ErrorDialog(Exception ex)
		{
			StringBuilder builder = new StringBuilder();
			while (ex != null)
			{
				builder.AppendLine(ex.Message);
				builder.AppendLine(ex.StackTrace);
				builder.AppendLine();
				ex = ex.InnerException;
			}

			this.TitleText = "An error has occurred. the application must quit.\nWe recommend you restart FFXIV.";
			this.MessageText = builder.ToString();

			this.InitializeComponent();
			this.ContentArea.DataContext = this;
		}

		public string TitleText { get; set; }
		public string MessageText { get; set; }

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			this.Close();
			Application.Current.Shutdown(2);
		}
	}
}
