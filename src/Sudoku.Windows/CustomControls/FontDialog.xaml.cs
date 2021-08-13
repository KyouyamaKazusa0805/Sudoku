namespace Sudoku.Windows.CustomControls;

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
	private readonly DBrush _brush = new SolidBrush(DColor.Black);


	/// <summary>
	/// Indicates whether the specified instance has been already disposed.
	/// </summary>
	private bool _isDisposed;


	/// <summary>
	/// Initializes a default <see cref="FontDialog"/> instance.
	/// </summary>
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
			new() { Alignment = Center, LineAlignment = Center }
		);

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
		// While initializing, 'SelectedFont' is null.
		if ((Sender: sender, SelectedFont) is (Sender: ListBox listBox, SelectedFont: not null))
		{
			SelectedFont = new(SelectedFont, (DFontStyle)listBox.SelectedIndex);

			DrawString();
		}
	}

	private void ListBoxFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		// While initializing, 'SelectedFont' is null.
		if ((Sender: sender, SelectedFont) is (Sender: ListBox listBox, SelectedFont: not null))
		{
			SelectedFont = new(listBox.SelectedItem.ToString()!, SelectedFont.Size, SelectedFont.Style);

			DrawString();
		}
	}

	private void TextBoxSize_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox)
		{
			e.Handled = true;
			return;
		}

		if (!double.TryParse(textBox.Text, out double value))
		{
			textBox.Text = "9";

			e.Handled = true;
			return;
		}

		SelectedFont = new(SelectedFont.Name, (float)value, SelectedFont.Style);

		DrawString();
	}

	private void TextBoxSize_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (sender is not TextBox textBox || !e.Key.IsDigit(false)
			|| Keyboard.Modifiers != ModifierKeys.None)
		{
			e.Handled = true;
			return;
		}

		textBox.Text = textBox.Text == "0"
			? e.Key.IsDigitUpsideAlphabets(false) ? (e.Key - D0).ToString() : (e.Key - NumPad0).ToString()
			: e.Key.IsDigitUpsideAlphabets() ? (e.Key - D0).ToString() : (e.Key - NumPad0).ToString();
	}
}
