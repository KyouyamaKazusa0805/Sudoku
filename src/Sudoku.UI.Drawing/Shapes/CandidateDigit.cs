namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the candidate-leveled digit.
/// The data structure represents for all possible candidates in a cell.
/// </summary>
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
internal sealed class CandidateDigit : DrawingElement
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
	/// Indicates the user preference.
	/// </summary>
	private readonly IDrawingPreference _preference;

	/// <summary>
	/// Indicates whether the current mode is mask mode.
	/// </summary>
	private bool _isMaskMode;

	/// <summary>
	/// Indicates whether the user controls on showing candidates.
	/// </summary>
	private bool _showsCandidates;

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
	/// <param name="preference">The user preference.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidateDigit(IDrawingPreference preference) : this(511, 0, preference)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CandidateDigit"/> instance via the details.
	/// </summary>
	/// <param name="candidateMask">The candidate mask.</param>
	/// <param name="wrongDigitMask">The wrong digits mask.</param>
	/// <param name="preference">The user preference.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="candidateMask"/> is negative number or greater than 511.
	/// </exception>
	public CandidateDigit(short candidateMask, short wrongDigitMask, IDrawingPreference preference) :
		this(candidateMask, wrongDigitMask, false, preference)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CandidateDigit"/> instance via the details.
	/// </summary>
	/// <param name="candidateMask">The candidate mask.</param>
	/// <param name="wrongDigitMask">The wrong digits mask.</param>
	/// <param name="maskMode">Whether the current mode is mask mode.</param>
	/// <param name="preference">The user preference.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="candidateMask"/> is negative number or greater than 511.
	/// </exception>
	public CandidateDigit(short candidateMask, short wrongDigitMask, bool maskMode, IDrawingPreference preference)
	{
		Argument.ThrowIfFalse(candidateMask is >= 0 and <= 511);
		Argument.ThrowIfFalse(wrongDigitMask is >= 0 and <= 511);

		(_isMaskMode, _candidateMask, _wrongDigitMask, _preference, _showsCandidates) = (
			maskMode,
			candidateMask,
			wrongDigitMask,
			preference,
			preference.ShowCandidates
		);

		var grid = new GridLayout { Visibility = preference.ShowCandidates ? Visibility.Visible : Visibility.Collapsed };
		grid.RowDefinitions.Add(new());
		grid.RowDefinitions.Add(new());
		grid.RowDefinitions.Add(new());
		grid.ColumnDefinitions.Add(new());
		grid.ColumnDefinitions.Add(new());
		grid.ColumnDefinitions.Add(new());

		for (byte digit = 0; digit < 9; digit++)
		{
			var digitBlock = new TextBlock
			{
				Text = (digit + 1).ToString(),
				FontFamily = new(_preference.CandidateFontName),
				FontSize = 60 * _preference.CandidateFontScale,
				Visibility = ((candidateMask | wrongDigitMask) >> digit & 1, maskMode) switch
				{
					(not 0, false) => Visibility.Visible,
					_ => Visibility.Collapsed
				},
				TextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				Foreground = new SolidColorBrush(
					(wrongDigitMask >> digit & 1) != 0 ? _preference.CandidateDeltaColor : _preference.CandidateColor
				)
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
		get => _preference.ShowCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_preference.ShowCandidates == value)
			{
				return;
			}

			_preference.ShowCandidates = value;
			_grid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	/// <summary>
	/// Gets or sets the value indicating whether user controls on displaying candidates.
	/// </summary>
	public bool UserShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _showsCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_showsCandidates == value)
			{
				return;
			}

			_showsCandidates = value;
			_grid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	/// <summary>
	/// Gets or sets the value indicating whether the current mode is mask mode.
	/// </summary>
	public bool IsMaskMode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isMaskMode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_isMaskMode == value)
			{
				return;
			}

			if (_isMaskMode && !ShowCandidates)
			{
				return;
			}

			_isMaskMode = value;
			_grid.Visibility = value || !_showsCandidates ? Visibility.Collapsed : Visibility.Visible;
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
			Argument.ThrowIfFalse(value is >= 0 and <= 511);

			_candidateMask = value;
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
			Argument.ThrowIfFalse(value is >= 0 and <= 511);

			_wrongDigitMask = value;
			for (byte digit = 0; digit < 9; digit++)
			{
				ref var current = ref _digitBlocks[digit];
				current.Visibility =
					((_candidateMask | value) >> digit & 1) != 0 ? Visibility.Visible : Visibility.Collapsed;
				current.Foreground = new SolidColorBrush(
					(_wrongDigitMask >> digit & 1) != 0
						? _preference.CandidateDeltaColor
						: _preference.CandidateColor
				);
			}
		}
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CandidateDigit);

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
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is CandidateDigit comparer && _candidateMask == comparer._candidateMask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _candidateMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _grid;
}
