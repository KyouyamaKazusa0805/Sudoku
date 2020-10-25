using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Constants;
using Sudoku.Extensions;

namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// Provides operations for grid formatting.
		/// </summary>
		public readonly ref struct GridFormatter
		{
			/// <summary>
			/// Initializes an instance with a <see cref="bool"/> value
			/// indicating multi-line.
			/// </summary>
			/// <param name="multiline">
			/// The multi-line identifier. If the value is <see langword="true"/>, the output will
			/// be multi-line.
			/// </param>
			public GridFormatter(bool multiline)
				: this('.', multiline, false, false, false, false, false, false, false)
			{
			}

			/// <summary>
			/// Initialize an instance with the specified information.
			/// </summary>
			/// <param name="placeholder">The placeholder.</param>
			/// <param name="multiline">Indicates whether the formatter will use multiple lines mode.</param>
			/// <param name="withModifiables">Indicates whether the formatter will output modifiables.</param>
			/// <param name="withCandidates">Indicates whether the formatter will output candidates list.</param>
			/// <param name="treatValueAsGiven">Indicates whether the formatter will treat values as givens always.</param>
			/// <param name="subtleGridLines">
			/// Indicates whether the formatter will process outline corner of the multiline grid.
			/// </param>
			/// <param name="hodokuCompatible">
			/// Indicates whether the formatter will use hodoku library mode to output.
			/// </param>
			/// <param name="sukaku">Indicates whether the formatter will output as sukaku.</param>
			/// <param name="excel">Indicates whether the formatter will output as excel.</param>
			private GridFormatter(
				char placeholder, bool multiline, bool withModifiables, bool withCandidates,
				bool treatValueAsGiven, bool subtleGridLines, bool hodokuCompatible, bool sukaku, bool excel)
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
			/// value at the tail of the string in ':candidate list'. In addition,
			/// candidates will be represented as 'digit', 'row offset' and
			/// 'column offset' in order.
			/// </para>
			/// </summary>
			public bool WithCandidates { get; init; }

			/// <summary>
			/// Indicates the output will treat modifiable values as given ones.
			/// If the output is single line, the output will remove all plus marks '+'.
			/// If the output is multi-line, the output will use '<c>&lt;digit&gt;</c>' instead
			/// of '<c>*digit*</c>'.
			/// </summary>
			public bool TreatValueAsGiven { get; init; }

			/// <summary>
			/// Indicates whether need to handle all grid outlines while outputting.
			/// See <a href="https://github.com/Sunnie-Shine/Sudoku/wiki/Grid-Format-String">this link</a>
			/// for more information.
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
			/// Get the default list.
			/// </summary>
			/// <returns>The list.</returns>
			private static IDictionary<int, IList<short>> DefaultList =>
				new Dictionary<int, IList<short>>()
				{
					[0] = new List<short>(),
					[1] = new List<short>(),
					[2] = new List<short>(),
					[3] = new List<short>(),
					[4] = new List<short>(),
					[5] = new List<short>(),
					[6] = new List<short>(),
					[7] = new List<short>(),
					[8] = new List<short>(),
				};

			/// <summary>
			/// Represents a string value indicating this instance.
			/// </summary>
			/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
			/// <returns>The string.</returns>
			public string ToString(in SudokuGrid grid) =>
				Sukaku
					? ToSukakuString(grid)
					: Multiline
						? WithCandidates
							? ToMultiLineStringCore(grid)
							: Excel ? ToExcelString(grid) : ToMultiLineSimpleGridCore(grid)
						: HodokuCompatible ? ToHodokuLibraryFormatString(grid) : ToSingleLineStringCore(grid);

			/// <summary>
			/// Represents a string value indicating this instance, with the specified format string.
			/// </summary>
			/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
			/// <param name="format">The string format.</param>
			/// <returns>The string.</returns>
			public string ToString(in SudokuGrid grid, string? format) => Create(format).ToString(grid);

			/// <summary>
			/// To Excel format string.
			/// </summary>
			/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
			/// <returns>The string.</returns>
			private string ToExcelString(in SudokuGrid grid)
			{
				var span = grid.ToString("0").AsSpan();
				var sb = new StringBuilder();
				for (int i = 0; i < 9; i++)
				{
					for (int j = 0; j < 9; j++)
					{
						sb.Append(span[i * 9 + j] - '0' is var digit and not 0 ? digit : '\t');
					}

					sb.RemoveFromEnd(1).AppendLine();
				}

				return sb.ToString();
			}

			/// <summary>
			/// To string with Hodoku library format compatible string.
			/// </summary>
			/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
			/// <returns>The string.</returns>
			private string ToHodokuLibraryFormatString(in SudokuGrid grid) =>
				$":0000:x:{ToSingleLineStringCore(grid)}:::";

			/// <summary>
			/// To string with the sukaku format.
			/// </summary>
			/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
			/// <returns>The string.</returns>
			/// <exception cref="ArgumentException">
			/// Throws when the puzzle is an invalid sukaku puzzle (at least one cell is given or modifiable).
			/// </exception>
			[SkipLocalsInit]
			private string ToSukakuString(in SudokuGrid grid)
			{
				if (Multiline)
				{
					// Append all digits.
					var builders = new StringBuilder[81];
					for (int i = 0; i < 81; i++)
					{
						builders[i] = new();
						foreach (int digit in grid.GetCandidates(i))
						{
							builders[i].Append(digit + 1);
						}
					}

					// Now consider the alignment for each column of output text.
					var sb = new StringBuilder();
					var span = (stackalloc int[9]);
					for (int column = 0; column < 9; column++)
					{
						int maxLength = 0;
						for (int p = 0; p < 9; p++)
						{
							maxLength = Math.Max(maxLength, builders[p * 9 + column].Length);
						}

						span[column] = maxLength;
					}
					for (int row = 0; row < 9; row++)
					{
						for (int column = 0; column < 9; column++)
						{
							int cell = row * 9 + column;
							sb.Append($"{builders[cell].ToString().PadLeft(span[column])} ");
						}
						sb.RemoveFromEnd(1).AppendLine(); // Remove last whitespace.
					}

					return sb.ToString();
				}
				else
				{
					var sb = new StringBuilder();
					for (int i = 0; i < 81; i++)
					{
						sb.Append("123456789");
					}

					for (int i = 0; i < 729; i++)
					{
						if (grid[i / 9, i % 9])
						{
							sb[i] = Placeholder;
						}
					}

					return sb.ToString();
				}
			}

			/// <summary>
			/// To single line string.
			/// </summary>
			/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
			/// <returns>The result.</returns>
			private string ToSingleLineStringCore(in SudokuGrid grid)
			{
				StringBuilder sb = new(), elims = new();
				var tempGrid = WithCandidates ? Parse(grid.ToString(".+")) : Undefined;

				int cell = 0;
				foreach (short value in grid)
				{
					if (GetStatusFromMask(value) is var status && status == CellStatus.Empty
						&& tempGrid != Undefined && WithCandidates)
					{
						// Check if the value has been set 'true'
						// and the value has already deleted at the grid
						// with only givens and modifiables.
						foreach (int i in from i in value where !tempGrid[cell, i] select i)
						{
							// The value is 'true', which means the digit has already been deleted.
							elims.Append($"{i + 1}{cell / 9 + 1}{cell % 9 + 1} ");
						}
					}

					sb.Append(
						status switch
						{
							CellStatus.Empty => Placeholder,
							CellStatus.Modifiable =>
								WithModifiables ? $"+{GetFirstFalseCandidate(value) + 1}" : $"{Placeholder}",
							CellStatus.Given => $"{GetFirstFalseCandidate(value) + 1}",
							_ => throw Throwings.ImpossibleCase
						});

					cell++;
				}

				string elimsStr = elims.Length <= 3 ? elims.ToString() : elims.RemoveFromEnd(1).ToString();
				return $"{sb}{(string.IsNullOrEmpty(elimsStr) ? string.Empty : $":{elimsStr}")}";
			}

			/// <summary>
			/// To multi-line string with candidates.
			/// </summary>
			/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
			/// <returns>The result.</returns>
			[SkipLocalsInit]
			private string ToMultiLineStringCore(in SudokuGrid grid)
			{
				// Step 1: gets the candidates information grouped by columns.
				var valuesByColumn = DefaultList;
				var valuesByRow = DefaultList;
				for (int i = 0; i < 81; i++)
				{
					short value = grid.GetMask(i);
					valuesByRow[i / 9].Add(value);
					valuesByColumn[i % 9].Add(value);
				}

				// Step 2: gets the maximal number of candidates in a cell,
				// which is used for aligning by columns.
				var maxLengths = (stackalloc int[9]);
				foreach (var (i, values) in valuesByColumn)
				{
					ref int maxLength = ref maxLengths[i];

					// Iteration on row index.
					for (int j = 0; j < 9; j++)
					{
						// Gets the number of candidates.
						int candidatesCount = 0;
						short value = valuesByColumn[i][j];

						// Iteration on each candidate.
						// Counts the number of candidates.
						candidatesCount += value.PopCount();

						// Compares the values.
						int comparer =
							Math.Max(
								candidatesCount,
								GetStatusFromMask(value) switch
								{
									// The output will be '<digit>' and consist of 3 characters.
									CellStatus.Given => Math.Max(candidatesCount, 3),
									// The output will be '*digit*' and consist of 3 characters.
									CellStatus.Modifiable => Math.Max(candidatesCount, 3),
									// Normal output: 'series' (at least 1 character).
									_ => candidatesCount,
								});
						if (comparer > maxLength)
						{
							maxLength = comparer;
						}
					}
				}

				// Step 3: outputs all characters.
				var sb = new StringBuilder();
				for (int i = 0; i < 13; i++)
				{
					switch (i)
					{
						case 0: // Print tabs of the first line.
						{
							if (SubtleGridLines) printTabLines('.', '.', '-', maxLengths);
							else printTabLines('+', '+', '-', maxLengths);
							break;
						}
						case 4 or 8: // Print tabs of mediate lines.
						{
							if (SubtleGridLines) printTabLines(':', '+', '-', maxLengths);
							else printTabLines('+', '+', '-', maxLengths);
							break;
						}
						case 12: // Print tabs of the foot line.
						{
							if (SubtleGridLines) printTabLines('\'', '\'', '-', maxLengths);
							else printTabLines('+', '+', '-', maxLengths);
							break;
						}
						default: // Print values and tabs.
						{
							p(
								this,
								valuesByRow
								[
									i switch
									{
										1 or 2 or 3 or 4 => i - 1,
										5 or 7 or 7 or 8 => i - 2,
										9 or 10 or 11 or 12 => i - 3,
										_ => throw Throwings.ImpossibleCaseWithMessage("On the border.")
									}
								], '|', '|', maxLengths);

							break;

							void p(
								in GridFormatter formatter, IList<short> valuesByRow, char c1, char c2,
								Span<int> maxLengths)
							{
								sb.Append(c1);
								printValues(formatter, valuesByRow, 0, 2, maxLengths);
								sb.Append(c2);
								printValues(formatter, valuesByRow, 3, 5, maxLengths);
								sb.Append(c2);
								printValues(formatter, valuesByRow, 6, 8, maxLengths);
								sb.AppendLine(c1);

								void printValues(
									in GridFormatter formatter, IList<short> valuesByRow,
									int start, int end, Span<int> maxLengths)
								{
									sb.Append(' ');
									for (int i = start; i <= end; i++)
									{
										// Get digit.
										short value = valuesByRow[i];
										var cellStatus = GetStatusFromMask(value);

										sb
											.Append
											(
												(
													((cellStatus != CellStatus.Empty ? (~value).FindFirstSet() : -1) + 1) switch
													{
														var d => cellStatus switch
														{
															CellStatus.Given => $"<{d}>",
															CellStatus.Modifiable => formatter.TreatValueAsGiven switch
															{
																true => $"<{d}>",
																_ => $"*{d}*"
															},
															_ => p(value)
														}
													}
												).PadRight(maxLengths[i])
											).Append(i != end ? "  " : " ");
									}

									static string p(short value)
									{
										var sb = new StringBuilder();
										foreach (int i in value)
										{
											sb.Append(i + 1);
										}

										return sb.ToString();
									}
								}
							}
						}
					}
				}

				// The last step: returns the value.
				return sb.ToString();

				void printTabLines(char c1, char c2, char fillingChar, in Span<int> maxLengths) =>
					sb
						.Append(c1)
						.Append(string.Empty.PadRight(maxLengths[0] + maxLengths[1] + maxLengths[2] + 6, fillingChar))
						.Append(c2)
						.Append(string.Empty.PadRight(maxLengths[3] + maxLengths[4] + maxLengths[5] + 6, fillingChar))
						.Append(c2)
						.Append(string.Empty.PadRight(maxLengths[6] + maxLengths[7] + maxLengths[8] + 6, fillingChar))
						.AppendLine(c1);
			}

			/// <summary>
			/// To multi-line normal grid string without any candidates.
			/// </summary>
			/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
			/// <returns>The result.</returns>
			private string ToMultiLineSimpleGridCore(in SudokuGrid grid)
			{
				string t = grid.ToString(TreatValueAsGiven ? $"{Placeholder}!" : $"{Placeholder}");
				return new StringBuilder()
					.AppendLine(SubtleGridLines ? ".-------+-------+-------." : "+-------+-------+-------+")
					.AppendLine($"| {t[0]} {t[1]} {t[2]} | {t[3]} {t[4]} {t[5]} | {t[6]} {t[7]} {t[8]} |")
					.AppendLine($"| {t[9]} {t[10]} {t[11]} | {t[12]} {t[13]} {t[14]} | {t[15]} {t[16]} {t[17]} |")
					.AppendLine($"| {t[18]} {t[19]} {t[20]} | {t[21]} {t[22]} {t[23]} | {t[24]} {t[25]} {t[26]} |")
					.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+")
					.AppendLine($"| {t[27]} {t[28]} {t[29]} | {t[30]} {t[31]} {t[32]} | {t[33]} {t[34]} {t[35]} |")
					.AppendLine($"| {t[36]} {t[37]} {t[38]} | {t[39]} {t[40]} {t[41]} | {t[42]} {t[43]} {t[44]} |")
					.AppendLine($"| {t[45]} {t[46]} {t[47]} | {t[48]} {t[49]} {t[50]} | {t[51]} {t[52]} {t[53]} |")
					.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+")
					.AppendLine($"| {t[54]} {t[55]} {t[56]} | {t[57]} {t[58]} {t[59]} | {t[60]} {t[61]} {t[62]} |")
					.AppendLine($"| {t[63]} {t[64]} {t[65]} | {t[66]} {t[67]} {t[68]} | {t[69]} {t[70]} {t[71]} |")
					.AppendLine($"| {t[72]} {t[73]} {t[74]} | {t[75]} {t[76]} {t[77]} | {t[78]} {t[79]} {t[80]} |")
					.AppendLine(SubtleGridLines ? "'-------+-------+-------'" : "+-------+-------+-------+")
					.ToString();
			}


			/// <summary>
			/// Create a <see cref="GridFormatter"/> according to the specified grid output options.
			/// </summary>
			/// <param name="gridOutputOption">The grid output options.</param>
			/// <returns>The grid formatter.</returns>
			public static GridFormatter Create(GridFormattingOptions gridOutputOption) =>
				gridOutputOption switch
				{
					GridFormattingOptions.Excel => new(true) { Excel = true },
					_ => new GridFormatter(gridOutputOption.Flags(GridFormattingOptions.Multiline))
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
			/// Create a <see cref="GridFormatter"/> according to the specified format.
			/// </summary>
			/// <param name="format">The format.</param>
			/// <returns>The grid formatter.</returns>
			/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
			public static GridFormatter Create(string? format) =>
				format switch
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
					_ => throw new FormatException("The specified format is invalid.")
				};

			/// <summary>
			/// Get the first <see langword="false"/> candidate.
			/// </summary>
			/// <param name="value">The value.</param>
			/// <returns>The first one.</returns>
			private static int GetFirstFalseCandidate(short value) =>
				(short)~(value & MaxCandidatesMask) is var v and not 0 ? v.FindFirstSet() : -1;

			/// <summary>
			/// Get cell status for a value.
			/// </summary>
			/// <param name="value">The value.</param>
			/// <returns>The cell status.</returns>
			private static CellStatus GetStatusFromMask(short value) =>
				(CellStatus)(value >> 9 & (int)CellStatus.All);
		}
	}
}
