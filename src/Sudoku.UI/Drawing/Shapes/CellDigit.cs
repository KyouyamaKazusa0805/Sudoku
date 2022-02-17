using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the cell-levelled digit.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
#endif
public sealed class CellDigit : DrawingElement
{
	/// <summary>
	/// The inner text block.
	/// </summary>
	private readonly TextBlock _textBlock;

	/// <summary>
	/// Indicates the cell status.
	/// </summary>
	private bool _isGiven;

	/// <summary>
	/// Indicates the font name.
	/// </summary>
	private string _fontName;

	/// <summary>
	/// Indicates the given color.
	/// </summary>
	private Color _givenColor;

	/// <summary>
	/// Indicates the modifiable color.
	/// </summary>
	private Color _modifiableColor;


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
	/// <param name="givenColor">The color for displaying the given cells.</param>
	/// <param name="modifiableColor">The color for displaying the modifiable cells.</param>
	/// <param name="fontName">The font name.</param>
	/// <param name="fontSize">The font size.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="digit"/> is not 255 and not in range 0 to 8.
	/// </exception>
	public CellDigit(
		byte digit, bool isGiven, Color givenColor, Color modifiableColor, string fontName, double fontSize)
	{
		if (digit is >= 9 and not byte.MaxValue)
		{
			throw new ArgumentOutOfRangeException(nameof(digit));
		}

		_isGiven = isGiven;
		_fontName = fontName;
		_givenColor = givenColor;
		_modifiableColor = modifiableColor;
		_textBlock = new()
		{
			Text = digit == byte.MaxValue ? string.Empty : digit.ToString(),
			FontSize = fontSize,
			FontFamily = new(fontName),
			HorizontalTextAlignment = TextAlignment.Center,
			TextAlignment = TextAlignment.Center,
			Foreground = new SolidColorBrush(isGiven ? givenColor : modifiableColor)
		};
	}


	/// <summary>
	/// Indicates whether the current cell is the given cell.
	/// </summary>
	public bool IsGiven
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isGiven;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_isGiven = value;
			_textBlock.Foreground = new SolidColorBrush(value ? _givenColor : _modifiableColor);
		}
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
		get => _textBlock.Text is var s and not "" ? byte.Parse(s) : byte.MaxValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _textBlock.Text = value == byte.MaxValue ? string.Empty : value.ToString();
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
		get => _fontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_fontName = value;
			_textBlock.FontFamily = new(value);
		}
	}

	/// <summary>
	/// The given color.
	/// </summary>
	public Color GivenColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _givenColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_givenColor = value;
			if (_isGiven)
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
		get => _modifiableColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_modifiableColor = value;
			if (!_isGiven)
			{
				_textBlock.Foreground = new SolidColorBrush(value);
			}
		}
	}

#if DEBUG
	/// <summary>
	/// Defines the debugger view.
	/// </summary>
	private string DebuggerDisplayView =>
		$"{nameof(CellDigit)} {{ Status = {
			(_isGiven ? "Given" : "Modifiable")}, Digit = {
			(_textBlock.Text is var s and not "" ? s : "<Empty>")} }}";
#endif


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is CellDigit comparer && _isGiven == comparer._isGiven && Digit == comparer.Digit;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(nameof(CellDigit), _isGiven, Digit);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TextBlock GetControl() => _textBlock;
}
