namespace Sudoku.Data;

partial struct Grid
{
	/// <summary>
	/// Provides operations for grid formatting.
	/// </summary>
	public readonly ref partial struct Formatter
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
		public Formatter(bool multiline) : this(
			placeholder: '.', multiline: multiline, withModifiables: false,
			withCandidates: false, treatValueAsGiven: false, subtleGridLines: false,
			hodokuCompatible: false, sukaku: false, excel: false, openSudoku: false
		)
		{
		}

		/// <summary>
		/// Initializes a <see cref="Formatter"/> instance using the specified mask storing all possible flags.
		/// </summary>
		/// <param name="flags">The flags.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Formatter(short flags) => _flags = flags;

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Formatter(
			char placeholder,
			bool multiline,
			bool withModifiables,
			bool withCandidates,
			bool treatValueAsGiven,
			bool subtleGridLines,
			bool hodokuCompatible,
			bool sukaku,
			bool excel,
			bool openSudoku
		)
		{
			_flags = placeholder switch { '.' => 0, '0' => 512 };
			_flags |= (short)(multiline ? 256 : 0);
			_flags |= (short)(withModifiables ? 128 : 0);
			_flags |= (short)(withCandidates ? 64 : 0);
			_flags |= (short)(treatValueAsGiven ? 32 : 0);
			_flags |= (short)(subtleGridLines ? 16 : 0);
			_flags |= (short)(hodokuCompatible ? 8 : 0);
			_flags |= (short)(sukaku ? 4 : 0);
			_flags |= (short)(excel ? 2 : 0);
			_flags |= (short)(openSudoku ? 1 : 0);
		}


		/// <summary>
		/// The place holder.
		/// </summary>
		/// <returns>The result placeholder text.</returns>
		/// <value>The value to assign. The value must be 46 (<c>'.'</c>) or 48 (<c>'0'</c>).</value>
		public char Placeholder
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (_flags >> 9 & 1) != 0 ? '.' : '0';

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags = value switch { '.' => (short)(_flags & 511 | 512), '0' => (short)(_flags & 511) };
		}

		/// <summary>
		/// Indicates whether the output should be multi-line.
		/// </summary>
		public bool Multiline
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (_flags >> 8 & 1) != 0;

#if false
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags |= (short)(value ? 256 : 0);
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
			get => (_flags >> 7 & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags |= (short)(value ? 128 : 0);
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
			get => (_flags >> 6 & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags |= (short)(value ? 64 : 0);
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
			get => (_flags >> 5 & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags |= (short)(value ? 32 : 0);
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
			get => (_flags >> 4 & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags |= (short)(value ? 16 : 0);
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
			get => (_flags >> 3 & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags |= (short)(value ? 8 : 0);
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
			get => (_flags >> 2 & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags |= (short)(value ? 4 : 0);
		}

		/// <summary>
		/// Indicates the output will be Excel format.
		/// </summary>
		/// <returns>The output will be Excel format.</returns>
		/// <value>A <see cref="bool"/> value to set.</value>
		public bool Excel
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (_flags >> 1 & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			init => _flags |= (short)(value ? 2 : 0);
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
		public string ToString(in Grid grid, string? format) => Create(format).ToString(grid);


		/// <summary>
		/// Create a <see cref="Formatter"/> according to the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>The grid formatter.</returns>
		/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Formatter Create(string? format) => format switch
		{
			null or "." => new(false),
			"+" or ".+" or "+." => new(false) { WithModifiables = true },
			"0" => new(false) { Placeholder = '0' },
			":" => new(false) { WithCandidates = true },
			"!" or ".!" or "!." => new(false) { WithModifiables = true, TreatValueAsGiven = true },
			"0!" or "!0" => new(false) { Placeholder = '0', WithModifiables = true, TreatValueAsGiven = true },
			".:" => new(false) { WithCandidates = true },
			"0:" => new(false) { Placeholder = '0', WithCandidates = true },
			"0+" or "+0" => new(false) { Placeholder = '0', WithModifiables = true },
			"+:" or "+.:" or ".+:" or "#" or "#." => new(false) { WithModifiables = true, WithCandidates = true },
			"0+:" or "+0:" or "#0" => new(false) { Placeholder = '0', WithModifiables = true, WithCandidates = true },
			".!:" or "!.:" => new(false) { WithModifiables = true, TreatValueAsGiven = true },
			"0!:" or "!0:" => new(false) { Placeholder = '0', WithModifiables = true, TreatValueAsGiven = true },
			"@" or "@." => new(true) { SubtleGridLines = true },
			"@0" => new(true) { Placeholder = '0', SubtleGridLines = true },
			"@!" or "@.!" or "@!." => new(true) { TreatValueAsGiven = true, SubtleGridLines = true },
			"@0!" or "@!0" => new(true) { Placeholder = '0', TreatValueAsGiven = true, SubtleGridLines = true },
			"@*" or "@.*" or "@*." => new(true),
			"@0*" or "@*0" => new(true) { Placeholder = '0' },
			"@!*" or "@*!" => new(true) { TreatValueAsGiven = true },
			"@:" => new(true) { WithCandidates = true, SubtleGridLines = true },
			"@:!" or "@!:" => new(true) { WithCandidates = true, TreatValueAsGiven = true, SubtleGridLines = true },
			"@*:" or "@:*" => new(true) { WithCandidates = true },
			"@!*:" or "@*!:" or "@!:*" or "@*:!" or "@:!*" or "@:*!" => new(true) { WithCandidates = true, TreatValueAsGiven = true },
			"~" or "~0" => new(false) { Sukaku = true, Placeholder = '0' },
			"~." => new(false) { Sukaku = true },
			"@~" or "~@" => new(true) { Sukaku = true },
			"@~0" or "@0~" or "~@0" or "~0@" => new(true) { Sukaku = true, Placeholder = '0' },
			"@~." or "@.~" or "~@." or "~.@" => new(true) { Sukaku = true },
			"%" => new(true) { Excel = true },
			"^" => new(false) { OpenSudoku = true },
			_ => throw new FormatException("The specified format is invalid.")
		};

		/// <summary>
		/// Create a <see cref="Formatter"/> according to the specified grid output options.
		/// </summary>
		/// <param name="gridOutputOption">The grid output options.</param>
		/// <returns>The grid formatter.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Formatter Create(GridFormattingOptions gridOutputOption) => gridOutputOption switch
		{
			GridFormattingOptions.Excel => new(true) { Excel = true },
			GridFormattingOptions.OpenSudoku => new(false) { OpenSudoku = true },
			_ => new(gridOutputOption.Flags(GridFormattingOptions.Multiline))
			{
				WithModifiables = gridOutputOption.Flags(GridFormattingOptions.WithModifiers),
				WithCandidates = gridOutputOption.Flags(GridFormattingOptions.WithCandidates),
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
}
