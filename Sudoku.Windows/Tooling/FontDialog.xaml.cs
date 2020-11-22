using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.DocComments;
using Sudoku.Drawing.Extensions;
using static System.Drawing.StringAlignment;
using static System.Drawing.Text.TextRenderingHint;
using static System.Windows.Input.Key;
using DFontStyle = System.Drawing.FontStyle;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>FontDialog.xaml</c>.
	/// </summary>
	public partial class FontDialog : Window, IDisposable
	{
		/// <summary>
		/// Indicates the sample text.
		/// </summary>
		private const string SampleText = "0123456789";


		/// <summary>
		/// The internal brush.
		/// </summary>
		private readonly Brush _brush = new SolidBrush(Color.Black);


		/// <summary>
		/// Indicates whether the specified instance has been already disposed.
		/// </summary>
		private bool _isDisposed;


		/// <inheritdoc cref="DefaultConstructor"/>
		public FontDialog() => InitializeComponent();


		/// <summary>
		/// Indicates the current selected font.
		/// </summary>
		public Font SelectedFont { get; private set; } = null!;


		/// <inheritdoc/>
		public void Dispose()
		{
			if (!_isDisposed)
			{
				_brush.Dispose();

				// Free unmanaged resources (unmanaged objects) and override finalizer.
				// Set large fields to null.
				_isDisposed = true;

				GC.SuppressFinalize(this);
			}
		}

		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			_listBoxFonts.ItemsSource = from font in new InstalledFontCollection().Families select font.Name;
			_listBoxFonts.SelectedIndex = 0;

			SelectedFont = new(_listBoxFonts.Items[0].ToString()!, (float)FontSize, DFontStyle.Regular);

			_textBoxSize.Text = FontSize.ToString();

			DrawString();
		}

		/// <summary>
		/// To draw the text.
		/// </summary>
		private void DrawString()
		{
			var bitmap = new Bitmap((int)_imagePreview.Width, (int)_imagePreview.Height);
			using var g = Graphics.FromImage(bitmap);
			g.TextRenderingHint = AntiAlias;
			g.DrawString(
				SampleText, SelectedFont, _brush, bitmap.Width >> 1, bitmap.Height >> 1,
				new() { Alignment = Center, LineAlignment = Center });

			_imagePreview.Source = bitmap.ToImageSource();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			_brush.Dispose();
			Close();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;

			_brush.Dispose();
			Close();
		}

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ListBoxStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// While initializing, 'SelectedFont' is null.
			if ((sender, SelectedFont) is (ListBox listBox, not null))
			{
				SelectedFont = new(SelectedFont, (DFontStyle)listBox.SelectedIndex);

				DrawString();
			}
		}

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ListBoxFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// While initializing, 'SelectedFont' is null.
			if ((sender, SelectedFont) is (ListBox listBox, not null))
			{
				SelectedFont = new(listBox.SelectedItem.ToString()!, SelectedFont.Size, SelectedFont.Style);

				DrawString();
			}
		}

		/// <inheritdoc cref="Events.TextChanged(object?, EventArgs)"/>
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

				SelectedFont = new(SelectedFont.Name, (float)value, SelectedFont.Style);

				DrawString();
			}
		}

		/// <inheritdoc cref="Events.PreviewKeyDown(object?, EventArgs)"/>
		private void TextBoxSize_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			static bool c(Key key) =>
				key is > D0 and <= D9 or > NumPad0 and <= NumPad9
				&& Keyboard.Modifiers == ModifierKeys.None;

			if (sender is TextBox textBox && c(e.Key))
			{
				textBox.Text = textBox.Text == "0"
					? e.Key is > D0 and <= D9 ? (e.Key - D0).ToString() : (e.Key - NumPad0).ToString()
					: e.Key is >= D0 and <= D9 ? (e.Key - D0).ToString() : (e.Key - NumPad0).ToString();
			}
		}
	}
}
