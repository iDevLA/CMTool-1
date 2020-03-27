﻿namespace ConceptMatrix.Controls
{
	using System;
	using System.ComponentModel;
	using System.Data;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;

	/// <summary>
	/// Interaction logic for NumberBox.xaml
	/// </summary>
	public partial class NumberBox : UserControl, INotifyPropertyChanged
	{
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(NumberBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnValueChanged)));
		public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(nameof(TickFrequency), typeof(double), typeof(NumberBox));

		private string inputString;

		public NumberBox()
		{
			this.InitializeComponent();
			this.Content.DataContext = this;
		}

		public double TickFrequency
		{
			get
			{
				return (double)this.GetValue(TickFrequencyProperty);
			}
			set
			{
				this.SetValue(TickFrequencyProperty, value);
			}
		}

		public double Minimum { get; set; } = 0;
		public double Maximum { get; set; } = 100;
		public bool Wrap { get; set; } = true;

		private Key keyHeld = Key.None;

		public string Text
		{
			get
			{
				return inputString;
			}

			set
			{
				this.inputString = value;

				double val = 0;
				if (double.TryParse(value, out val))
				{
					this.Value = val;
					this.ErrorDisplay.Visibility = Visibility.Collapsed;
				}
				else
				{
					

					this.ErrorDisplay.Visibility = Visibility.Visible;
				}
			}
		}

		public double Value 
		{ 
			get
			{
				return (double)this.GetValue(ValueProperty);
			}

			set
			{
				value = Math.Round(value / this.TickFrequency) * this.TickFrequency;

				this.SetValue(ValueProperty, value);
				this.Text = this.Value.ToString("0.###");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is NumberBox numberBox)
			{
				numberBox.Text = numberBox.Value.ToString("0.##");
			}
		}

		private void OnLostFocus(object sender, RoutedEventArgs e)
		{
			this.Commit(false);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			bool focused = this.InputBox.IsKeyboardFocused || this.InputSlider.IsKeyboardFocused;
			if (!focused)
				return;

			if (e.Key == Key.Return)
			{
				this.Commit(true);
				e.Handled = true;
			}

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				e.Handled = true;

				if (e.IsRepeat)
				{
					if (this.keyHeld == e.Key)
						return;

					this.keyHeld = e.Key;
					Task.Run(this.TickHeldKey);
				}
				else
				{
					TickKey(e.Key);
				}
			}
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			if (this.keyHeld == e.Key)
			{
				e.Handled = true;
				this.keyHeld = Key.None;
			}
		}

		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			e.Handled = true;
			this.TickValue(e.Delta > 0);
		}

		private void Commit(bool refocus)
		{
			this.Value = Convert.ToDouble(new DataTable().Compute(this.inputString, null));
			this.Text = this.Value.ToString("0.##");

			if (refocus)
			{
				this.InputBox.Focus();
				this.InputBox.CaretIndex = int.MaxValue;
			}
		}

		private async Task TickHeldKey()
		{
			while (this.keyHeld != Key.None)
			{
				await Application.Current.Dispatcher.InvokeAsync(() =>
				{
					this.TickKey(keyHeld);
				});

				await Task.Delay(10);
			}
		}

		private void TickKey(Key key)
		{
			if (key == Key.Up)
			{
				this.TickValue(true);
				Commit(true);

			}
			else if (key == Key.Down)
			{
				this.TickValue(false);
				Commit(true);
			}
		}

		private void TickValue(bool increase)
		{
			double delta = increase ? TickFrequency : -TickFrequency;

			if (Keyboard.IsKeyDown(Key.LeftShift))
				delta *= 10;

			double value = this.Value;
			value += delta;

			if (this.Wrap)
			{
				while (value > this.Maximum)
				{
					value -= this.Maximum;
				}

				while (value < this.Minimum)
				{
					value += this.Maximum;
				}
			}
			else
			{
				value = Math.Min(value, this.Maximum);
				value = Math.Max(value, this.Minimum);
			}

			this.Value = value;
		}
	}
}
