using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the cell-levelled digit.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
#endif
internal sealed class CellDigit : DrawingElement
{
	/// <summary>
	/// The inner text block.
	/// </summary>
	private readonly TextBlock _textBlock;

	/// <summary>
	/// The user preference.
	/// </summary>
	private readonly UserPreference _userPreference;

	/// <summary>
	/// Indicates the cell status.
	/// </summary>
	private bool? _isGiven;


	/// <summary>
	/// Initializes a <see cref="CellDigit"/> instance via the details.
	/// </summary>
	/// <param name="userPreference">The user preference.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellDigit(UserPreference userPreference) : this(byte.MaxValue, false, userPreference)
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
	/// <param name="userPreference">The user preference.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="digit"/> is not 255 and not in range 0 to 8.
	/// </exception>
	public CellDigit(byte digit, bool? isGiven, UserPreference userPreference)
	{
		if (digit is >= 9 and not byte.MaxValue)
		{
			throw new ArgumentOutOfRangeException(nameof(digit));
		}

		_userPreference = userPreference;
		_isGiven = isGiven;
		_textBlock = new()
		{
			Text = digit == byte.MaxValue ? string.Empty : (digit + 1).ToString(),
			FontSize = userPreference.ValueFontSize,
			FontFamily = new(userPreference.ValueFontName),
			TextAlignment = TextAlignment.Center,
			HorizontalTextAlignment = TextAlignment.Center,
			Foreground = new SolidColorBrush(isGiven switch
			{
				true => userPreference.GivenColor,
				false => userPreference.ModifiableColor,
				_ => userPreference.CellDeltaColor
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
		set
		{
			_isGiven = value;
			_textBlock.Foreground = new SolidColorBrush(value switch
			{
				true => _userPreference.GivenColor,
				false => _userPreference.ModifiableColor,
				_ => _userPreference.CellDeltaColor
			});
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
		get => _userPreference.ValueFontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_userPreference.ValueFontName = value;
			_textBlock.FontFamily = new(value);
		}
	}

	/// <summary>
	/// The given color.
	/// </summary>
	public Color GivenColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _userPreference.GivenColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_userPreference.GivenColor = value;
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
		get => _userPreference.ModifiableColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_userPreference.ModifiableColor = value;
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
		get => _userPreference.CellDeltaColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_userPreference.CellDeltaColor = value;
			if (_isGiven is null)
			{
				_textBlock.Foreground = new SolidColorBrush(value);
			}
		}
	}

#if DEBUG
	/// <summary>
	/// Defines the debugger view.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private string DebuggerDisplayView =>
		$$"""
		{{
			nameof(CellDigit)}} { Status = {{
			(_isGiven is true ? "Given" : "Modifiable")}}, Digit = {{
			(_textBlock.Text is var s and not "" ? s : "<Empty>")}} }
		""";
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
