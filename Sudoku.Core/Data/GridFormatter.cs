using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Constants;
using Sudoku.Extensions;
using static System.Math;

namespace Sudoku.Data
{
	/// <summary>
	/// Provides operations for grid formatting.
	/// </summary>
	[Obsolete("Please use '" + nameof(SudokuGrid.GridFormatter) + "' instead.", true)]
	internal sealed partial class GridFormatter
	{
		/// <summary>
		/// Initializes an instance with a <see cref="bool"/> value
		/// indicating multi-line.
		/// </summary>
		/// <param name="multiline">
		/// The multi-line identifier. If the value is <see langword="true"/>, the output will
		/// be multi-line.
		/// </param>
		public GridFormatter(bool multiline) => Multiline = multiline;


		/// <summary>
		/// Indicates whether the output should be multi-line.
		/// </summary>
		public bool Multiline { get; }


		/// <summary>
		/// Represents a string value indicating this instance.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The string.</returns>
		public string ToString(Grid grid) =>
			Sukaku
				? ToSukakuString(grid)
				: Multiline
					? WithCandidates
						? ToMultiLineStringCore(grid)
						: Excel ? ToExcelString(grid) : ToMultiLineSimpleGridCore(grid)
					: HodokuCompatible ? ToHodokuLibraryFormatString(grid) : ToSingleLineStringCore(grid);

		/// <summary>
		/// To Excel format string.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The string.</returns>
		private string ToExcelString(Grid grid)
		{
			var span = $"{grid:0}".AsSpan();
			var sb = new StringBuilder();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					int digit = span[i * 9 + j] - '0';
					if (digit != 0)
					{
						sb.Append(digit);
					}

					sb.Append('\t');
				}

				sb.RemoveFromEnd(1).AppendLine();
			}

			return sb.ToString();
		}

		/// <summary>
		/// To string with Hodoku library format compatible string.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The string.</returns>
		private string ToHodokuLibraryFormatString(Grid grid) => $":0000:x:{ToSingleLineStringCore(grid)}:::";


		/// <summary>
		/// To string with the sukaku format.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The string.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when the puzzle is an invalid sukaku puzzle (at least one cell is given or modifiable).
		/// </exception>
		private string ToSukakuString(Grid grid)
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
						maxLength = Max(maxLength, builders[p * 9 + column].Length);
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
		/// <param name="grid">The grid.</param>
		/// <returns>The result.</returns>
		private string ToSingleLineStringCore(Grid grid)
		{
			var sb = new StringBuilder();
			var elims = new StringBuilder();
			var tempGrid = WithCandidates ? Grid.Parse($"{grid:.+}") : null;

			int offset = 0;
			foreach (short value in grid)
			{
				switch (GetCellStatus(value))
				{
					case CellStatus.Empty when tempGrid is not null && WithCandidates:
					{
						// Check if the value has been set 'true'
						// and the value has already deleted at the grid
						// with only givens and modifiables.
						foreach (int i in from i in value where !tempGrid[offset, i] select i)
						{
							// The value is 'true', which means the digit has already been deleted.
							elims.Append($"{i + 1}{offset / 9 + 1}{offset % 9 + 1} ");
						}

						goto case CellStatus.Empty;
					}
					case CellStatus.Empty:
					{
						sb.Append(Placeholder);
						break;
					}
					case CellStatus.Modifiable:
					{
						sb.Append(WithModifiables ? $"+{GetFirstFalseCandidate(value) + 1}" : $"{Placeholder}");

						break;
					}
					case CellStatus.Given:
					{
						sb.Append($"{GetFirstFalseCandidate(value) + 1}");
						break;
					}
					default:
					{
						break;
					}
				}

				offset++;
			}

			string elimsStr = elims.Length <= 3 ? elims.ToString() : elims.RemoveFromEnd(1).ToString();
			return $"{sb}{(string.IsNullOrEmpty(elimsStr) ? string.Empty : $":{elimsStr}")}";
		}

		/// <summary>
		/// Get the first <see langword="false"/> candidate.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The first one.</returns>
		private static int GetFirstFalseCandidate(short value)
		{
			// To truncate the value to range 0 to 511.
			value = (short)~(value & Grid.MaxCandidatesMask);
			return value != 0 ? value.FindFirstSet() : -1;
		}

		/// <summary>
		/// To multi-line normal grid string without any candidates.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The result.</returns>
		private string ToMultiLineSimpleGridCore(Grid grid)
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
		/// To multi-line string with candidates.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The result.</returns>
		private string ToMultiLineStringCore(Grid grid)
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
					for (int k = 0, copy = value; k < 9; k++, copy >>= 1)
					{
						if ((copy & 1) == 0)
						{
							candidatesCount++;
						}
					}

					// Compares the values.
					int comparer = Max(
						candidatesCount,
						GetCellStatus(value) switch
						{
							// The output will be '<digit>' and consist of 3 characters.
							CellStatus.Given => Max(candidatesCount, 3),
							// The output will be '*digit*' and consist of 3 characters.
							CellStatus.Modifiable => Max(candidatesCount, 3),
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

						void p(IList<short> valuesByRow, char c1, char c2, Span<int> maxLengths)
						{
							sb.Append(c1);
							printValues(valuesByRow, 0, 2, maxLengths);
							sb.Append(c2);
							printValues(valuesByRow, 3, 5, maxLengths);
							sb.Append(c2);
							printValues(valuesByRow, 6, 8, maxLengths);
							sb.AppendLine(c1);

							void printValues(IList<short> valuesByRow, int start, int end, Span<int> maxLengths)
							{
								sb.Append(" ");
								for (int i = start; i <= end; i++)
								{
									// Get digit.
									short value = valuesByRow[i];
									var cellStatus = GetCellStatus(value);
									int digit = cellStatus != CellStatus.Empty ? (~value).FindFirstSet() : -1;

									sb
										.Append(
										(
											(digit + 1) switch
											{
												var d => cellStatus switch
												{
													CellStatus.Given => $"<{d}>",
													CellStatus.Modifiable => TreatValueAsGiven ? $"<{d}>" : $"*{d}*",
													_ => p(value)
												}
											}
										).PadRight(maxLengths[i]))
										.Append(i != end ? "  " : " ");
								}

								static string p(short value)
								{
									var sb = new StringBuilder();
									for (int i = 1; i <= 9; i++, value >>= 1)
									{
										if ((value & 1) == 0)
										{
											sb.Append(i);
										}
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

			void printTabLines(char c1, char c2, char fillingChar, Span<int> maxLengths) =>
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
		/// Get cell status for a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The cell status.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static CellStatus GetCellStatus(short value) => (CellStatus)(value >> 9 & (int)CellStatus.All);

		/// <summary>
		/// Get the default list.
		/// </summary>
		/// <returns>The list.</returns>
		private static Dictionary<int, List<short>> DefaultList =>
			new()
			{
				[0] = new(),
				[1] = new(),
				[2] = new(),
				[3] = new(),
				[4] = new(),
				[5] = new(),
				[6] = new(),
				[7] = new(),
				[8] = new(),
			};
	}
}
