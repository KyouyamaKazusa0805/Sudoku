using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.UI.Xaml.Media;
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
	/// Indicates the inner grid.
	/// </summary>
	private readonly GridLayout _grid;

	/// <summary>
	/// Indicates the user preference.
	/// </summary>
	private readonly UserPreference _userPreference;

	/// <summary>
	/// Indicates the digit blocks.
	/// </summary>
	private readonly TextBlock[] _digitBlocks = new TextBlock[9];

	/// <summary>
	/// Indicates the candidate mask.
	/// </summary>
	private short _candidateMask;

	/// <summary>
	/// Indicates the wrong digit mask.
	/// </summary>
	private short _wrongDigitMask;


	/// <summary>
	/// Initializes a <see cref="CandidateDigit"/> instance via the details.
	/// </summary>
	/// <param name="userPreference">The user preference.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidateDigit(UserPreference userPreference) : this(511, 0, userPreference)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CandidateDigit"/> instance via the details.
	/// </summary>
	/// <param name="candidateMask">The candidate mask.</param>
	/// <param name="wrongDigitMask">The wrong digits mask.</param>
	/// <param name="userPreference">The user preference.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="candidateMask"/> is negative number or greater than 511.
	/// </exception>
	public CandidateDigit(short candidateMask, short wrongDigitMask, UserPreference userPreference)
	{
		_candidateMask = candidateMask is >= 0 and <= 511
			? candidateMask
			: throw new ArgumentOutOfRangeException(nameof(candidateMask));
		_wrongDigitMask = wrongDigitMask is >= 0 and <= 511
			? wrongDigitMask
			: throw new ArgumentOutOfRangeException(nameof(wrongDigitMask));

		_userPreference = userPreference;
		var grid = new GridLayout
		{
			Visibility = userPreference.ShowCandidates ? Visibility.Visible : Visibility.Collapsed
		}.WithRowDefinitionsCount(3).WithColumnDefinitionsCount(3);
		for (byte digit = 0; digit < 9; digit++)
		{
			var digitBlock = new TextBlock
			{
				Text = (digit + 1).ToString(),
				FontFamily = new(_userPreference.CandidateFontName),
				FontSize = _userPreference.CandidateFontSize,
				Visibility = ((candidateMask | wrongDigitMask) >> digit & 1) != 0 ? Visibility.Visible : Visibility.Collapsed,
				TextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				Foreground = new SolidColorBrush(
					(wrongDigitMask >> digit & 1) != 0
						? _userPreference.CandidateDeltaColor
						: _userPreference.CandidateColor)
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
	public bool ShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _userPreference.ShowCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_userPreference.ShowCandidates == value)
			{
				return;
			}

			_userPreference.ShowCandidates = value;
			_grid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
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
				_digitBlocks[digit].Visibility =
					((value | _wrongDigitMask) >> digit & 1) != 0 ? Visibility.Visible : Visibility.Collapsed;
			}
		}
	}

	/// <summary>
	/// Indicates the wrong digits mask.
	/// </summary>
	public short WrongDigitMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _wrongDigitMask;

		set
		{
			_wrongDigitMask = value is >= 0 and <= 511
				? value
				: throw new ArgumentOutOfRangeException(nameof(value));

			for (byte digit = 0; digit < 9; digit++)
			{
				ref var current = ref _digitBlocks[digit];
				current.Visibility =
					((_candidateMask | value) >> digit & 1) != 0 ? Visibility.Visible : Visibility.Collapsed;
				current.Foreground = new SolidColorBrush(
					(_wrongDigitMask >> digit & 1) != 0
						? _userPreference.CandidateDeltaColor
						: _userPreference.CandidateColor);
			}
		}
	}

#if DEBUG
	/// <summary>
	/// Defines the debugger view.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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
