using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Forms.Tooling
{
	/// <summary>
	/// Interaction logic for ColorDialog.xaml
	/// </summary>
	public partial class ColorDialog : Window
	{
		/// <summary>
		/// The default brush.
		/// </summary>
		private readonly Brush _brush = new SolidBrush(Color.Black);


		public ColorDialog() => InitializeComponent();


		public Color SelectedColor { get; private set; }


		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			SelectedColor = Color.FromArgb(255, Color.Black);
			_textBoxA.Text = 255.ToString();
			_textBoxR.Text = _textBoxG.Text = _textBoxB.Text = 0.ToString();

			UpdatePreview();
		}

		private void UpdatePreview()
		{
			var bitmap = new Bitmap((int)_imagePreview.Width, (int)_imagePreview.Height);
			using var g = Graphics.FromImage(bitmap);
			g.TextRenderingHint = TextRenderingHint.AntiAlias;
			g.DrawString(
				"sudoku", new Font("Times New Roman", 16F), _brush,
				bitmap.Width >> 1, bitmap.Height >> 1, new StringFormat
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center
				});
			g.Clear(SelectedColor);

			_imagePreview.Source = bitmap.ToImageSource();

			GC.Collect();
		}


		private void TextBoxA_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!int.TryParse(textBox.Text, out int value))
				{
					e.Handled = true;
					return;
				}

				if (value < 0 || value > 255)
				{
					_textBoxA.Text = (value = 255).ToString();
				}

				SelectedColor = Color.FromArgb(value, SelectedColor);
			}
		}

		private void TextBoxR_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!int.TryParse(textBox.Text, out int value))
				{
					e.Handled = true;
					return;
				}

				if (value < 0 || value > 255)
				{
					_textBoxR.Text = (value = 255).ToString();
				}

				SelectedColor = Color.FromArgb(SelectedColor.A, value, SelectedColor.G, SelectedColor.B);
			}
		}

		private void TextBoxG_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!int.TryParse(textBox.Text, out int value))
				{
					e.Handled = true;
					return;
				}

				if (value < 0 || value > 255)
				{
					_textBoxG.Text = (value = 255).ToString();
				}

				SelectedColor = Color.FromArgb(SelectedColor.A, SelectedColor.R, value, SelectedColor.B);
			}
		}

		private void TextBoxB_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!int.TryParse(textBox.Text, out int value))
				{
					e.Handled = true;
					return;
				}

				if (value < 0 || value > 255)
				{
					_textBoxB.Text = (value = 255).ToString();
				}

				SelectedColor = Color.FromArgb(SelectedColor.A, SelectedColor.R, SelectedColor.G, value);
			}
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			_brush.Dispose();
			Close();
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;

			_brush.Dispose();
			Close();
		}

		private void TextBoxA_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Back && e.Key != Key.Delete && !(
				e.Key >= Key.D0 && e.Key <= Key.D9
				|| e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
			{
				e.Handled = true;
				return;
			}
		}

		private void TextBoxR_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Back && e.Key != Key.Delete && !(
				e.Key >= Key.D0 && e.Key <= Key.D9
				|| e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
			{
				e.Handled = true;
				return;
			}
		}

		private void TextBoxG_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Back && e.Key != Key.Delete && !(
				e.Key >= Key.D0 && e.Key <= Key.D9
				|| e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
			{
				e.Handled = true;
				return;
			}
		}

		private void TextBoxB_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Back && e.Key != Key.Delete && !(
				e.Key >= Key.D0 && e.Key <= Key.D9
				|| e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
			{
				e.Handled = true;
				return;
			}
		}
	}
}
