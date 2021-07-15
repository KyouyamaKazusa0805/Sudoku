using System;
using System.Extensions;

namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// Provides operations for grid formatting.
		/// </summary>
		public readonly ref partial struct Formatter
		{
			/// <summary>
			/// Initializes an instance with a <see cref="bool"/> value
			/// indicating multi-line.
			/// </summary>
			/// <param name="multiline">
			/// The multi-line identifier. If the value is <see langword="true"/>, the output will
			/// be multi-line.
			/// </param>
			public Formatter(bool multiline) : this(
				placeholder: '.',
				multiline: multiline,
				withModifiables: false,
				withCandidates: false,
				treatValueAsGiven: false,
				subtleGridLines: false,
				hodokuCompatible: false,
				sukaku: false,
				excel: false,
				openSudoku: false)
			{
			}

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
			private Formatter(
				char placeholder, bool multiline, bool withModifiables, bool withCandidates,
				bool treatValueAsGiven, bool subtleGridLines, bool hodokuCompatible, bool sukaku,
				bool excel, bool openSudoku)
			{
				Placeholder = placeholder;
				Multiline = multiline;
				WithModifiables = withModifiables;
				WithCandidates = withCandidates;
				TreatValueAsGiven = treatValueAsGiven;
				SubtleGridLines = subtleGridLines;
				HodokuCompatible = hodokuCompatible;
				Sukaku = sukaku;
				Excel = excel;
				OpenSudoku = openSudoku;
			}


			/// <summary>
			/// The place holder.
			/// </summary>
			public char Placeholder { get; init; }

			/// <summary>
			/// Indicates whether the output should be multi-line.
			/// </summary>
			public bool Multiline { get; }

			/// <summary>
			/// Indicates the output should be with modifiable values.
			/// </summary>
			public bool WithModifiables { get; init; }

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
			public bool WithCandidates { get; init; }

			/// <summary>
			/// Indicates the output will treat modifiable values as given ones.
			/// If the output is single line, the output will remove all plus marks '+'.
			/// If the output is multi-line, the output will use '<c><![CDATA[<digit>]]></c>' instead
			/// of '<c>*digit*</c>'.
			/// </summary>
			public bool TreatValueAsGiven { get; init; }

			/// <summary>
			/// Indicates whether need to handle all grid outlines while outputting.
			/// </summary>
			public bool SubtleGridLines { get; init; }

			/// <summary>
			/// Indicates whether the output will be compatible with Hodoku library format.
			/// </summary>
			public bool HodokuCompatible { get; init; }

			/// <summary>
			/// Indicates the output will be sukaku format (all single-valued digit will
			/// be all treated as candidates).
			/// </summary>
			public bool Sukaku { get; init; }

			/// <summary>
			/// Indicates the output will be Excel format.
			/// </summary>
			public bool Excel { get; init; }

			/// <summary>
			/// Indicates whether the current output mode is aiming to open sudoku format.
			/// </summary>
			public bool OpenSudoku { get; init; }


			/// <summary>
			/// Represents a string value indicating this instance.
			/// </summary>
			/// <param name="grid">The grid.</param>
			/// <returns>The string.</returns>
			public string ToString(in SudokuGrid grid) => Sukaku
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
			public string ToString(in SudokuGrid grid, string? format) => Create(format).ToString(grid);


			/// <summary>
			/// Create a <see cref="Formatter"/> according to the specified grid output options.
			/// </summary>
			/// <param name="gridOutputOption">The grid output options.</param>
			/// <returns>The grid formatter.</returns>
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

			/// <summary>
			/// Create a <see cref="Formatter"/> according to the specified format.
			/// </summary>
			/// <param name="format">The format.</param>
			/// <returns>The grid formatter.</returns>
			/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
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


			private partial string ToExcelString(in SudokuGrid grid);
			private partial string ToOpenSudokuString(in SudokuGrid grid);
			private partial string ToHodokuLibraryFormatString(in SudokuGrid grid);
			private partial string ToSukakuString(in SudokuGrid grid);
			private partial string ToSingleLineStringCore(in SudokuGrid grid);
			private partial string ToMultiLineStringCore(in SudokuGrid grid);
			private partial string ToMultiLineSimpleGridCore(in SudokuGrid grid);
		}
	}
}
