namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a unknown value view node shape.
/// </summary>
public sealed class UnknownValueViewNodeShape : DrawingElement
{
	/// <summary>
	/// The inner control.
	/// </summary>
	private readonly TextBlock _textBlock = new TextBlock()
		.WithFontWeight(FontWeights.Bold)
		.WithFontStyle(FontStyle.Italic)
		.WithHorizontalAlignment(HorizontalAlignment.Center)
		.WithVerticalAlignment(VerticalAlignment.Center)
		.WithTextAlignment(TextAlignment.Center)
		.WithHorizontalTextAlignment(TextAlignment.Center);

	/// <summary>
	/// Indicates the preference.
	/// </summary>
	private readonly IDrawingPreference _preference = null!;

	/// <summary>
	/// The character.
	/// </summary>
	private Utf8Char _char;


	/// <summary>
	/// Indicates the unknown character used. If you want to hide the control, assign <c>(Utf8Char)'\0'</c>.
	/// </summary>
	public Utf8Char UnknownCharacter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Utf8Char)_textBlock.Text[0];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_char = value;
			if (value == (byte)'\0')
			{
				_textBlock.Visibility = Visibility.Collapsed;
			}
			else
			{
				_textBlock.Visibility = Visibility.Visible;
				_textBlock.Text = value.ToString();
			}
		}
	}

	/// <summary>
	/// Indicates the preference instance.
	/// </summary>
	public required IDrawingPreference Preference
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init
		{
			_preference = value;

			UnknownCharacter = (Utf8Char)'\0';
			_textBlock
				.WithFontFamily(value.UnknownValueFont.FontName)
				.WithFontSize(60 * value.UnknownValueFont.FontScale);
		}
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(UnknownValueViewNodeShape);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is UnknownValueViewNodeShape comparer && _char == comparer._char;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _char);

	/// <inheritdoc/>
	public override TextBlock GetControl() => _textBlock;
}
