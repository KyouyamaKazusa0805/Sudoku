namespace Sudoku.Concepts.Formatting;

/// <summary>
/// Provides a formatter that gathers the main information for a <see cref="Grid"/> instance,
/// and convert it to a <see cref="string"/> value as the result.
/// </summary>
public readonly ref partial struct GridFormatter
{
	/// <summary>
	/// Indicates the inner mask that stores the flags.
	/// </summary>
	private readonly short _flags;


	/// <summary>
	/// Initializes an instance with a <see cref="bool"/> value
	/// indicating multi-line.
	/// </summary>
	/// <param name="multiline">
	/// The multi-line identifier. If the value is <see langword="true"/>, the output will
	/// be multi-line.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridFormatter(bool multiline) : this(
		placeholder: '.', multiline: multiline, withModifiables: false,
		withCandidates: false, treatValueAsGiven: false, subtleGridLines: false,
		hodokuCompatible: false, sukaku: false, excel: false, openSudoku: false,
		shortenSusser: false)
	{
	}

	/// <summary>
	/// Initializes a <see cref="GridFormatter"/> instance using the specified mask storing all possible flags.
	/// </summary>
	/// <param name="flags">The flags.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridFormatter(short flags) => _flags = flags;

	/// <summary>
	/// Initialize an instance with the specified information.
	/// </summary>
	/// <param name="placeholder">The placeholder.</param>
	/// <param name="multiline">Indicates whether the formatter will use multiple lines mode.</param>
	/// <param name="withModifiables">Indicates whether the formatter will output modifiables.</param>
	/// <param name="withCandidates">
	/// Indicates whether the formatter will output candidates list.
	/// </param>
	/// <param name="treatValueAsGiven">
	/// Indicates whether the formatter will treat values as givens always.
	/// </param>
	/// <param name="subtleGridLines">
	/// Indicates whether the formatter will process outline corner of the multiline grid.
	/// </param>
	/// <param name="hodokuCompatible">
	/// Indicates whether the formatter will use hodoku library mode to output.
	/// </param>
	/// <param name="sukaku">Indicates whether the formatter will output as sukaku.</param>
	/// <param name="excel">Indicates whether the formatter will output as excel.</param>
	/// <param name="openSudoku">
	/// Indicates whether the formatter will output as open sudoku format.
	/// </param>
	/// <param name="shortenSusser">
	/// Indicates whether the formatter will shorten the susser format.
	/// </param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="placeholder"/> is not supported.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private GridFormatter(
		char placeholder, bool multiline, bool withModifiables, bool withCandidates,
		bool treatValueAsGiven, bool subtleGridLines, bool hodokuCompatible,
		bool sukaku, bool excel, bool openSudoku, bool shortenSusser)
	{
		_flags = placeholder switch
		{
			'.' => 0,
			'0' => 1024,
			_ => throw new ArgumentOutOfRangeException(nameof(placeholder))
		};
		_flags |= (short)(multiline ? 512 : 0);
		_flags |= (short)(withModifiables ? 256 : 0);
		_flags |= (short)(withCandidates ? 128 : 0);
		_flags |= (short)(treatValueAsGiven ? 64 : 0);
		_flags |= (short)(subtleGridLines ? 32 : 0);
		_flags |= (short)(hodokuCompatible ? 16 : 0);
		_flags |= (short)(sukaku ? 8 : 0);
		_flags |= (short)(excel ? 4 : 0);
		_flags |= (short)(openSudoku ? 2 : 0);
		_flags |= (short)(shortenSusser ? 1 : 0);
	}


	/// <summary>
	/// The place holder.
	/// </summary>
	/// <returns>The result placeholder text.</returns>
	/// <value>The value to assign. The value must be 46 (<c>'.'</c>) or 48 (<c>'0'</c>).</value>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <see langword="value"/> is not supported.
	/// </exception>
	public char Placeholder
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 10 & 1) != 0 ? '.' : '0';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags = value switch
		{
			'.' => (short)(_flags & 1023 | 1024),
			'0' => (short)(_flags & 1023),
			_ => throw new ArgumentOutOfRangeException(nameof(value))
		};
	}

	/// <summary>
	/// Indicates whether the output should be multi-line.
	/// </summary>
	public bool Multiline
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 9 & 1) != 0;

#if false
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 512 : 0);
#endif
	}

	/// <summary>
	/// Indicates the output should be with modifiable values.
	/// </summary>
	/// <returns>The output should be with modifiable values.</returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool WithModifiables
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 8 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 256 : 0);
	}

	/// <summary>
	/// <para>
	/// Indicates the output should be with candidates.
	/// If the output is single line, the candidates will indicate
	/// the candidates-have-eliminated before the current grid status;
	/// if the output is multi-line, the candidates will indicate
	/// the real candidate at the current grid status.
	/// </para>
	/// <para>
	/// If the output is single line, the output will append the candidates
	/// value at the tail of the string in '<c>:candidate list</c>'. In addition,
	/// candidates will be represented as 'digit', 'row offset' and
	/// 'column offset' in order.
	/// </para>
	/// </summary>
	/// <returns>The output should be with candidates.</returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool WithCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 7 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 128 : 0);
	}

	/// <summary>
	/// Indicates the output will treat modifiable values as given ones.
	/// If the output is single line, the output will remove all plus marks '+'.
	/// If the output is multi-line, the output will use '<c><![CDATA[<digit>]]></c>' instead
	/// of '<c>*digit*</c>'.
	/// </summary>
	/// <returns>The output will treat modifiable values as given ones.</returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool TreatValueAsGiven
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 6 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 64 : 0);
	}

	/// <summary>
	/// Indicates whether need to handle all grid outlines while outputting.
	/// </summary>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether need to handle all grid outlines while outputting.
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool SubtleGridLines
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 5 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 32 : 0);
	}

	/// <summary>
	/// Indicates whether the output will be compatible with Hodoku library format.
	/// </summary>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the output will be compatible
	/// with Hodoku library format.
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool HodokuCompatible
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 4 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 16 : 0);
	}

	/// <summary>
	/// Indicates the output will be sukaku format (all single-valued digit will
	/// be all treated as candidates).
	/// </summary>
	/// <returns>
	/// The output will be sukaku format (all single-valued digit will
	/// be all treated as candidates).
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool Sukaku
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 3 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 8 : 0);
	}

	/// <summary>
	/// Indicates the output will be Excel format.
	/// </summary>
	/// <returns>The output will be Excel format.</returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool Excel
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 2 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 4 : 0);
	}

	/// <summary>
	/// Indicates whether the current output mode is aiming to open sudoku format.
	/// </summary>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the current output mode
	/// is aiming to open sudoku format.
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool OpenSudoku
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 1 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 2 : 0);
	}

	/// <summary>
	/// Indicates whether the current output mode will shorten the susser format.
	/// </summary>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the current output mode
	/// will shorten the susser format.
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool ShortenSusser
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 1 : 0);
	}


	/// <summary>
	/// Represents a string value indicating this instance.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(in Grid grid) =>
		Sukaku
			? ToSukakuString(grid)
			: Multiline
				? WithCandidates
					? ToMultiLineStringCore(grid)
					: Excel
						? ToExcelString(grid)
						: ToMultiLineSimpleGridCore(grid)
				: HodokuCompatible
					? ToHodokuLibraryFormatString(grid)
					: OpenSudoku
						? ToOpenSudokuString(grid)
						: ToSingleLineStringCore(grid);

	/// <summary>
	/// Represents a string value indicating this instance, with the specified format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="format">The string format.</param>
	/// <returns>The string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(in Grid grid, string? format) => GridFormatterFactory.Create(format).ToString(grid);


	/// <summary>
	/// Create a <see cref="GridFormatter"/> according to the specified grid output options.
	/// </summary>
	/// <param name="gridOutputOption">The grid output options.</param>
	/// <returns>The grid formatter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GridFormatter Create(GridFormattingOptions gridOutputOption) =>
		gridOutputOption switch
		{
			GridFormattingOptions.Excel => new(true) { Excel = true },
			GridFormattingOptions.OpenSudoku => new(false) { OpenSudoku = true },
			_ => new(gridOutputOption.Flags(GridFormattingOptions.Multiline))
			{
				WithModifiables = gridOutputOption.Flags(GridFormattingOptions.WithModifiers),
				WithCandidates = gridOutputOption.Flags(GridFormattingOptions.WithCandidates),
				ShortenSusser = gridOutputOption.Flags(GridFormattingOptions.Shorten),
				TreatValueAsGiven = gridOutputOption.Flags(GridFormattingOptions.TreatValueAsGiven),
				SubtleGridLines = gridOutputOption.Flags(GridFormattingOptions.SubtleGridLines),
				HodokuCompatible = gridOutputOption.Flags(GridFormattingOptions.HodokuCompatible),
				Sukaku = gridOutputOption == GridFormattingOptions.Sukaku,
				Placeholder = gridOutputOption.Flags(GridFormattingOptions.DotPlaceholder) ? '.' : '0'
			}
		};


	private partial string ToExcelString(in Grid grid);
	private partial string ToOpenSudokuString(in Grid grid);
	private partial string ToHodokuLibraryFormatString(in Grid grid);
	private partial string ToSukakuString(in Grid grid);
	private partial string ToSingleLineStringCore(in Grid grid);
	private partial string ToMultiLineStringCore(in Grid grid);
	private partial string ToMultiLineSimpleGridCore(in Grid grid);
}
