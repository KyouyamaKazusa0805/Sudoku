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
		.WithHorizontalTextAlignment(TextAlignment.Center)
		.WithOpacity(1)
		.WithOpacityTransition(TimeSpan.FromMilliseconds(500));

	/// <summary>
	/// Indicates the preference.
	/// </summary>
	private readonly IDrawingPreference _preference = null!;

	/// <summary>
	/// The character.
	/// </summary>
	private Utf8Char _char;


	/// <summary>
	/// Indicates the unknown character used. If you want to hide the control, assign <c>(Utf8Char)'\0'</c>
	/// or call method <see cref="SetVisibilityCollapsed"/>.
	/// </summary>
	/// <seealso cref="SetVisibilityCollapsed"/>
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
				_textBlock.Opacity = 0;
			}
			else
			{
				_textBlock.Opacity = 1;
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
				.WithFontSize(value.RenderingCellSize * value.UnknownValueFont.FontScale)
				.WithForeground(value.UnknownValueColor);
		}
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(UnknownValueViewNodeShape);


	/// <summary>
	/// Sets the visibility to collapsed. This operation is equal to assigning <c>(Utf8Char)'\0'</c>
	/// to the property <see cref="UnknownCharacter"/>.
	/// </summary>
	/// <seealso cref="UnknownCharacter"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetVisibilityCollapsed() => UnknownCharacter = (Utf8Char)'\0';

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is UnknownValueViewNodeShape comparer && _char == comparer._char;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _char);

	/// <inheritdoc/>
	public override TextBlock GetControl() => _textBlock;
}
