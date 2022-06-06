namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the cell-levelled digit.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
#endif
[AutoOverridesGetHashCode(nameof(TypeIdentifier), nameof(_isGiven), nameof(Digit))]
internal sealed partial class CellDigit : DrawingElement
{
	/// <summary>
	/// Indicates the cell status.
	/// </summary>
	private bool? _isGiven;

	/// <summary>
	/// The inner text block.
	/// </summary>
	private readonly TextBlock _textBlock;

	/// <summary>
	/// The user preference.
	/// </summary>
	private readonly IDrawingPreference _preference;


	/// <summary>
	/// Initializes a <see cref="CellDigit"/> instance via the details.
	/// </summary>
	/// <param name="preference">The user preference.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellDigit(IDrawingPreference preference) : this(byte.MaxValue, false, preference)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CellDigit"/> instance via the details.
	/// </summary>
	/// <param name="digit">
	/// The digit value. If you want to hide the value, just assign 255;
	/// otherwise, using 0 to 8 to indicate the displaying value corresponding to the real digit 1 to 9.
	/// </param>
	/// <param name="isGiven">
	/// Indicates whether the cell is given. If <see langword="false"/>, modifiable value.
	/// </param>
	/// <param name="preference">The user preference.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="digit"/> is not 255 and not in range 0 to 8.
	/// </exception>
	public CellDigit(byte digit, bool? isGiven, IDrawingPreference preference)
	{
		Argument.ThrowIfFalse(digit is < 9 or byte.MaxValue);

		(_preference, _isGiven) = (preference, isGiven);
		_textBlock = new()
		{
			Text = digit == byte.MaxValue ? string.Empty : (digit + 1).ToString(),
			FontSize = 60 * preference.ValueFontScale,
			FontFamily = new(preference.ValueFontName),
			TextAlignment = TextAlignment.Center,
			HorizontalTextAlignment = TextAlignment.Center,
			Foreground = new SolidColorBrush(isGiven switch
			{
				true => preference.GivenColor,
				false => preference.ModifiableColor,
				_ => preference.CellDeltaColor
			})
		};
	}


	/// <summary>
	/// Indicates whether the current cell is the given cell.
	/// </summary>
	/// <returns>
	/// The return value is <see cref="bool"/>?, which means the return value contains three possible cases:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The cell is the given.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The cell is modifiable.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The cell is modifiable, but the value is wrong.</description>
	/// </item>
	/// </list>
	/// </returns>
	public bool? IsGiven
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isGiven;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => (_isGiven, _textBlock.Foreground) = (
			value,
			new SolidColorBrush(
				value switch
				{
					true => _preference.GivenColor,
					false => _preference.ModifiableColor,
					_ => _preference.CellDeltaColor
				}
			)
		);
	}

	/// <summary>
	/// Indicates whether the current element is shown.
	/// </summary>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <see langword="value"/> holds <see langword="true"/> value.
	/// </exception>
	public bool IsShown
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Digit != 255;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Digit = value ? throw new ArgumentException("The value must be false.", nameof(value)) : byte.MaxValue;
	}

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _textBlock.Text is var s and not "" ? (byte)(byte.Parse(s) - 1) : byte.MaxValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _textBlock.Text = value == byte.MaxValue ? string.Empty : (value + 1).ToString();
	}

	/// <summary>
	/// The font size.
	/// </summary>
	public double FontSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _textBlock.FontSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _textBlock.FontSize = value;
	}

	/// <summary>
	/// The font name.
	/// </summary>
	public string FontName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.ValueFontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_preference.ValueFontName = value;
			_textBlock.FontFamily = new(value);
		}
	}

	/// <summary>
	/// The given color.
	/// </summary>
	public Color GivenColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.GivenColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_preference.GivenColor = value;
			if (_isGiven is true)
			{
				_textBlock.Foreground = new SolidColorBrush(value);
			}
		}
	}

	/// <summary>
	/// The modifiable color.
	/// </summary>
	public Color ModifiableColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.ModifiableColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_preference.ModifiableColor = value;
			if (_isGiven is false)
			{
				_textBlock.Foreground = new SolidColorBrush(value);
			}
		}
	}

	/// <summary>
	/// Indicates the cell delta color.
	/// </summary>
	public Color CellDeltaColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.CellDeltaColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_preference.CellDeltaColor = value;
			if (_isGiven is null)
			{
				_textBlock.Foreground = new SolidColorBrush(value);
			}
		}
	}

	private string TypeIdentifier => nameof(CellDigit);

#if DEBUG
	/// <summary>
	/// Defines the debugger view.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private string DebuggerDisplayView
		=> $$"""
		{{
			nameof(CellDigit)}} { Status = {{
			(_isGiven is true ? "Given" : "Modifiable")}}, Digit = {{
			(_textBlock.Text is var s and not "" ? s : "<Empty>")}} }
		""";
#endif


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is CellDigit comparer && _isGiven == comparer._isGiven && Digit == comparer.Digit;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TextBlock GetControl() => _textBlock;
}
