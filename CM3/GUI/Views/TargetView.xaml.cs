// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI.Views
{
	using System;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using ConceptMatrix.Offsets;
	using ConceptMatrix.Services;

	/// <summary>
	/// Interaction logic for TargetView.xaml.
	/// </summary>
	public partial class TargetView : UserControl
	{
		public TargetView()
		{
			this.InitializeComponent();
			Task.Run(this.Cheat);
		}

		public async Task Cheat()
		{
			try
			{
				await Task.Delay(100);

				while (!App.Services.IsStarted)
					await Task.Delay(100);

				IInjectionService injection = App.Services.Get<IInjectionService>();
				while (true)
				{
					await Task.Delay(500);

					IMemory<string> name = injection.GetMemory<string>(BaseAddresses.GPose, injection.Offsets.Character.Name);

					Application.Current.Dispatcher.Invoke(() =>
					{
						this.NameLabel.Content = name.Get();
					});
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex);
			}
		}
	}
}
