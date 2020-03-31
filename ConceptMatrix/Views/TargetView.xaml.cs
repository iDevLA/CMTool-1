// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI.Views
{
	using System.Windows;
	using System.Windows.Controls;
	using ConceptMatrix.Services;

	/// <summary>
	/// Interaction logic for TargetView.xaml.
	/// </summary>
	public partial class TargetView : UserControl
	{
		public TargetView()
		{
			this.InitializeComponent();

			ISelectionService selection = App.Services.Get<ISelectionService>();
			selection.SelectionChanged += this.OnSelectionChanged;
			this.OnSelectionChanged(selection.CurrentSelection);
		}

		private void OnSelectionChanged(Selection args)
		{
			if (args == null)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					this.NameLabel.Content = string.Empty;
				});
			}
			else
			{
				IInjectionService injection = App.Services.Get<IInjectionService>();
				IMemory<string> name = injection.GetMemory<string>(args.BaseAddress, injection.Offsets.Character.Name);

				Application.Current.Dispatcher.Invoke(() =>
				{
					this.NameLabel.Content = name.Get();
				});
			}
		}
	}
}
