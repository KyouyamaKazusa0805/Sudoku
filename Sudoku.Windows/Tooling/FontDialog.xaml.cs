using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.Drawing.Extensions;
using d = System.Drawing;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for FontDialog.xaml
	/// </summary>
	public partial class FontDialog : Window
	{
		/// <summary>
		/// The internal brush.
		/// </summary>
		private readonly Brush _brush = new SolidBrush(Color.Black);


		public FontDialog() => InitializeComponent();


		public Font SelectedFont { get; private set; } = null!;


		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			foreach (var font in new InstalledFontCollection().Families)
			{
				_listBoxFonts.Items.Add(font.Name);
			}
			_listBoxFonts.SelectedIndex = 0;

			SelectedFont = new Font(
				_listBoxFonts.Items[0].ToString(), (float)FontSize, (d::FontStyle)0);

			_listBoxStyle.Items.Add("Normal");
			_listBoxStyle.Items.Add("Italic");
			_listBoxStyle.Items.Add("Bold");
			_listBoxStyle.Items.Add("Bold Italic");
			_listBoxStyle.SelectedIndex = 0;

			_textBoxSize.Text = FontSize.ToString();

			DrawString();
		}

		private void DrawString()
		{
			var bitmap = new Bitmap((int)_imagePreview.Width, (int)_imagePreview.Height);
			using var g = Graphics.FromImage(bitmap);
			g.TextRenderingHint = TextRenderingHint.AntiAlias;
			g.DrawString(
				"0123456789", SelectedFont, _brush, bitmap.Width / 2, bitmap.Height / 2,
				new StringFormat
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center
				});

			_imagePreview.Source = bitmap.ToImageSource();
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

		private void ListBoxStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox)
			{
				if (SelectedFont is null)
				{
					// While initializing, 'SelectedFont' is null.
					e.Handled = true;
					return;
				}

				SelectedFont = new Font(
					SelectedFont, listBox.SelectedIndex switch
					{
						0 => d::FontStyle.Regular,
						1 => d::FontStyle.Bold,
						2 => d::FontStyle.Italic,
						3 => d::FontStyle.Bold | d::FontStyle.Italic,
						_ => throw Throwing.ImpossibleCase
					});

				DrawString();
			}
		}

		private void ListBoxFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox)
			{
				if (SelectedFont is null)
				{
					// While initializing, 'SelectedFont' is null.
					e.Handled = true;
					return;
				}

				SelectedFont = new Font(
					listBox.SelectedItem.ToString(), SelectedFont.Size, SelectedFont.Style);

				DrawString();
			}
		}

		private void TextBoxSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!double.TryParse(textBox.Text, out double value))
				{
					textBox.Text = "9";

					e.Handled = true;
					return;
				}

				SelectedFont = new Font(SelectedFont.Name, (float)value, SelectedFont.Style);

				DrawString();
			}
		}

		private void TextBoxSize_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (textBox.Text == "0")
				{
					if (!(e.Key > Key.D0 && e.Key <= Key.D9 || e.Key > Key.NumPad0 && e.Key <= Key.NumPad9)
						|| Keyboard.Modifiers != ModifierKeys.None)
					{
						e.Handled = true;
						return;
					}

					textBox.Text = e.Key > Key.D0 && e.Key <= Key.D9
						? (e.Key - Key.D0).ToString()
						: (e.Key - Key.NumPad0).ToString();
				}
				else
				{
					if (!(e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
						|| Keyboard.Modifiers != ModifierKeys.None)
					{
						e.Handled = true;
						return;
					}

					textBox.Text = e.Key >= Key.D0 && e.Key <= Key.D9
						? (e.Key - Key.D0).ToString()
						: (e.Key - Key.NumPad0).ToString();
				}
			}
		}
	}
}
