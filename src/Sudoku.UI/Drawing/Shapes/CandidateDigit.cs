using Microsoft.UI.Xaml.Media;
using Windows.UI;
using static System.Numerics.BitOperations;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the candidate-levelled digit.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
#endif
public sealed class CandidateDigit : DrawingElement
{
	/// <summary>
	/// Indicates the inner grid.
	/// </summary>
	private readonly GridLayout _grid;

	/// <summary>
	/// Indicates the digit blocks.
	/// </summary>
	private readonly TextBlock[] _digitBlocks = new TextBlock[9];

	/// <summary>
	/// Indicates the candidate mask.
	/// </summary>
	private ushort _candidateMask;


	/// <summary>
	/// Initializes a <see cref="CandidateDigit"/> instance via the details.
	/// </summary>
	/// <param name="candidateMask">The candidate mask.</param>
	/// <param name="color">The color.</param>
	public CandidateDigit(ushort candidateMask, Color color)
	{
		_candidateMask = candidateMask;

		var grid = new GridLayout();
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
				Text = digit.ToString(),
				Visibility = containsTheDigit ? Visibility.Visible : Visibility.Collapsed,
				Foreground = new SolidColorBrush(color)
			};

			GridLayout.SetRow(digitBlock, digit / 3);
			GridLayout.SetColumn(digitBlock, digit % 3);

			_digitBlocks[digit] = digitBlock;
		}

		_grid = grid;
	}


	/// <summary>
	/// Indicates the candidate mask.
	/// </summary>
	public ushort CandidateMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateMask;

		set
		{
			if (value >= 511)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			_candidateMask = value;

			for (byte digit = 0; digit < 9; digit++)
			{
				bool containsTheDigit = (value >> digit & 1) != 0;
				_digitBlocks[digit].Visibility = containsTheDigit ? Visibility.Visible : Visibility.Collapsed;
			}
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
			int c = PopCount(_candidateMask);
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
