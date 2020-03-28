// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.PoseModule
{
	using System.Threading;
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Interaction logic for CharacterPoseView.xaml.
	/// </summary>
	public partial class SimplePoseView : UserControl
	{
		public SimplePoseView()
		{
			this.InitializeComponent();
			Application.Current.Exit += this.OnApplicationExiting;

			ThreadStart ts = new ThreadStart(this.PollChanges);
			Thread th = new Thread(ts);
			th.Start();
		}

		public SimplePoseViewModel ViewModel { get; set; }

		private void PollChanges()
		{
			while (this.IsVisible)
			{
				Thread.Sleep(32);

				if (!this.ViewModel.IsEnabled)
					continue;

				if (this.ViewModel.CurrentBone == null)
					continue;

				this.ViewModel.CurrentBone.SetRotation();
			}
		}

		/*private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.DataContext is CharacterDetails details)
			{
				this.ViewModel = new SimplePoseViewModel(details);
				this.ContentArea.DataContext = this.ViewModel;
			}
		}*/

		private void OnApplicationExiting(object sender, ExitEventArgs e)
		{
			if (this.ViewModel == null)
				return;

			this.ViewModel.IsEnabled = false;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			if (this.ViewModel == null)
				return;

			this.ViewModel.IsEnabled = false;
		}
	}
}
