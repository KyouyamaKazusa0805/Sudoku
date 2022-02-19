using Microsoft.UI.Xaml.Media;
using Windows.UI;
using static System.Numerics.BitOperations;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the candidate-levelled digit.
/// The data structure represents for all possible candidates in a cell.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
#endif
internal sealed class CandidateDigit : DrawingElement
{
	/// <summary>
	/// Indicates whether the candidate block shows digits.
	/// </summary>
	private bool _showDigits;

	/// <summary>
	/// Indicates the font size of each digit.
	/// </summary>
	private double _fontSize;

	/// <summary>
	/// Indicates the candidate mask.
	/// </summary>
	private short _candidateMask;

	/// <summary>
	/// Indicates the font name of each digit.
	/// </summary>
	private string _fontName;

	/// <summary>
	/// Indicates the inner grid.
	/// </summary>
	private readonly GridLayout _grid;

	/// <summary>
	/// Indicates the digit blocks.
	/// </summary>
	private readonly TextBlock[] _digitBlocks = new TextBlock[9];


	/// <summary>
	/// Initializes a <see cref="CandidateDigit"/> instance via the details.
	/// </summary>
	/// <param name="showDigits">Indicates whether the candidate block shows the digits.</param>
	/// <param name="fontName">The font name.</param>
	/// <param name="fontSize">The font size.</param>
	/// <param name="color">The color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidateDigit(bool showDigits, string fontName, double fontSize, Color color) :
		this(0, showDigits, fontName, fontSize, color)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CandidateDigit"/> instance via the details.
	/// </summary>
	/// <param name="candidateMask">The candidate mask.</param>
	/// <param name="showDigits">Indicates whether the candidate block shows the digits.</param>
	/// <param name="fontName">The font name.</param>
	/// <param name="fontSize">The font size.</param>
	/// <param name="color">The color.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="candidateMask"/> is negative number or greater than 511.
	/// </exception>
	public CandidateDigit(short candidateMask, bool showDigits, string fontName, double fontSize, Color color)
	{
		_candidateMask = candidateMask is >= 0 and <= 511
			? candidateMask
			: throw new ArgumentOutOfRangeException(nameof(candidateMask));

		_showDigits = showDigits;
		_fontName = fontName;
		_fontSize = fontSize;

		var grid = new GridLayout { Visibility = showDigits ? Visibility.Visible : Visibility.Collapsed };
		grid.RowDefinitions.Add(new());
		grid.RowDefinitions.Add(new());
		grid.RowDefinitions.Add(new());
		grid.ColumnDefinitions.Add(new());
		grid.ColumnDefinitions.Add(new());
		grid.ColumnDefinitions.Add(new());
		for (byte digit = 0; digit < 9; digit++)
		{
			bool containsTheDigit = (candidateMask >> digit & 1) != 0;
			var digitBlock = new TextBlock
			{
				Text = (digit + 1).ToString(),
				FontFamily = new(fontName),
				FontSize = fontSize,
				Visibility = containsTheDigit ? Visibility.Visible : Visibility.Collapsed,
				TextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				Foreground = new SolidColorBrush(color)
			};

			GridLayout.SetRow(digitBlock, digit / 3);
			GridLayout.SetColumn(digitBlock, digit % 3);
			_digitBlocks[digit] = digitBlock;
			grid.Children.Add(digitBlock);
		}

		_grid = grid;
	}


	/// <summary>
	/// Gets or sets the value indicating whether the candidate block shows digits.
	/// </summary>
	public bool ShowDigits
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _showDigits;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_showDigits == value)
			{
				return;
			}

			_showDigits = value;
			_grid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	/// <summary>
	/// Gets or sets the font size.
	/// </summary>
	public double FontSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _fontSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_fontSize.NearlyEquals(value))
			{
				return;
			}

			_fontSize = value;
			Array.ForEach(_digitBlocks, digitBlock => digitBlock.FontSize = value);
		}
	}

	/// <summary>
	/// Indicates the candidate mask.
	/// </summary>
	public short CandidateMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateMask;

		set
		{
			_candidateMask = value is >= 0 and <= 511
				? value
				: throw new ArgumentOutOfRangeException(nameof(value));

			for (byte digit = 0; digit < 9; digit++)
			{
				bool containsTheDigit = (value >> digit & 1) != 0;
				_digitBlocks[digit].Visibility = containsTheDigit ? Visibility.Visible : Visibility.Collapsed;
			}
		}
	}

	/// <summary>
	/// Gets or sets the font name.
	/// </summary>
	public string FontName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _fontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_fontName == value)
			{
				return;
			}

			_fontName = value;
			Array.ForEach(_digitBlocks, digitBlock => digitBlock.FontFamily = new(value));
		}
	}

	/// <summary>
	/// Indicates the color.
	/// </summary>
	public Color Color
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((SolidColorBrush)_digitBlocks[0].Foreground).Color;

		set
		{
			for (byte digit = 0; digit < 9; digit++)
			{
				_digitBlocks[digit].Foreground = new SolidColorBrush(value);
			}
		}
	}

#if DEBUG
	/// <summary>
	/// Defines the debugger view.
	/// </summary>
	private string DebuggerDisplayView
	{
		get
		{
			int c = PopCount((uint)_candidateMask);
			var sb = new StringHandler();

			byte i = 0;
			foreach (int digit in _candidateMask)
			{
				sb.Append(digit);
				if (++i != c)
				{
					sb.Append(", ");
				}
			}

			return $"{nameof(CandidateDigit)} {{ Digits = {sb.ToStringAndClear()} }}";
		}
	}
#endif


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is CandidateDigit comparer && _candidateMask == comparer._candidateMask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(nameof(CandidateDigit), _candidateMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _grid;
}
