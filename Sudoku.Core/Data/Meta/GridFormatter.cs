using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Provides operations for grid formatting.
	/// </summary>
	[DebuggerStepThrough]
	internal sealed class GridFormatter
	{
		/// <summary>
		/// Initializes an instance with grid and is-multiline <see cref="bool"/> value.
		/// </summary>
		/// <param name="grid">A grid.</param>
		/// <param name="multiline">
		/// The multiline identifier. If the value is <see langword="true"/>, the output will
		/// be multiline.
		/// </param>
		public GridFormatter(Grid grid, bool multiline) =>
			(Grid, Multiline) = (grid, multiline);


		/// <summary>
		/// The place holder.
		/// </summary>
		public char Placeholder { get; set; } = '.';

		/// <summary>
		/// Indicates whether the output should be multiline.
		/// </summary>
		public bool Multiline { get; }

		/// <summary>
		/// Indicates the output should be with modifiable values.
		/// </summary>
		public bool WithModifiables { get; set; }

		/// <summary>
		/// <para>
		/// Indicates the output should be with candidates.
		/// If the output is single line, the candidates will indicate
		/// the candidates-have-eliminated before the current grid status;
		/// if the output is multiline, the candidates will indicate
		/// the real candidate at the current grid status.
		/// </para>
		/// <para>
		/// If the output is single line, the output will append the candidates
		/// value at the tail of the string in ':candidate list'. In addtition,
		/// candidates will be represented as 'digit', 'row offset' and
		/// 'column offset' in order.
		/// </para>
		/// </summary>
		public bool WithCandidates { get; set; }

		/// <summary>
		/// Indicates the output will treat modifiable values as given ones.
		/// If the output is single line, the output will remove all plus marks '+'.
		/// If the output is multiline, the output will use '&lt;digit&gt;' instead
		/// of '*digit*'.
		/// </summary>
		public bool TreatValueAsGiven { get; set; }

		/// <summary>
		/// Indicates whether need to handle all grid outlines when outputing.
		/// See <a href="https://github.com/Sunnie-Shine/Sudoku#multiline-format-characters">this link</a>
		/// for more information.
		/// </summary>
		public bool SubtleGridLines { get; set; }

		/// <summary>
		/// The grid.
		/// </summary>
		public Grid Grid { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			return Multiline
				? WithCandidates
					? ToMultiLineStringCore()
					: ToMultiLineSimpleGridCore()
				: ToSingleLineStringCore();
		}

		/// <summary>
		/// To single line string.
		/// </summary>
		/// <returns>The result.</returns>
		private string ToSingleLineStringCore()
		{
			static int GetFirstFalseCandidate(short value)
			{
				// To truncate the value to range 0 to 511.
				value = (short)~(value & 511);

				// Special case: the value is 0.
				if (value == 0)
				{
					return -1;
				}
				else
				{
					for (int i = 0; i < 9; i++, value >>= 1)
					{
						if ((value & 1) != 0)
						{
							return i;
						}
					}

					// All values are 0, which means the value is 0,
					// so return -1 is necessary, even though the special case has been
					// handled before.
					return -1;
				}
			}

			var sb = new StringBuilder();
			var elims = new StringBuilder();
			Grid tempGrid = null!; // This assignment is very dangerous (Non-nullable is assigned null)!
			if (WithCandidates)
			{
				// Get a temp grid only used for checking.
				tempGrid = Grid.Parse(Grid.ToString(".+"));
			}

			int offset = 0;
			foreach (short value in Grid)
			{
				switch (GetCellStatus(value))
				{
					case CellStatus.Empty:
					{
						if (WithCandidates)
						{
							for (int i = 0, temp = value; i < 9; i++, temp >>= 1)
							{
								// Check if the value has been set 'true'
								// and the value has already deleted at the grid
								// with only givens and modifiables.
								if ((temp & 1) != 0 && !tempGrid[offset, i])
								{
									// The value is 'true', which means that
									// The digit has been deleted.
									elims.Append($"{i + 1}{offset / 9 + 1}{offset % 9 + 1} ");
								}
							}
						}

						sb.Append(Placeholder);
						break;
					}
					case CellStatus.Modifiable:
					{
						sb.Append(
							WithModifiables
								? $"+{GetFirstFalseCandidate(value) + 1}"
								: $"{Placeholder}");

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

			string elimsStr = elims.ToString();
			return $"{sb}{(string.IsNullOrEmpty(elimsStr) ? string.Empty : $":{elimsStr}")}";
		}

		/// <summary>
		/// To multiline normal grid string without any candidates.
		/// </summary>
		/// <returns>The result.</returns>
		private string ToMultiLineSimpleGridCore()
		{
			string temp = Grid.ToString(TreatValueAsGiven ? $"{Placeholder}!" : $"{Placeholder}");
			var sb = new StringBuilder();
			sb.AppendLine(SubtleGridLines ? ".-------+-------+-------." : "+-------+-------+-------+");
			sb.AppendLine($"| {temp[0]} {temp[1]} {temp[2]} | {temp[3]} {temp[4]} {temp[5]} | {temp[6]} {temp[7]} {temp[8]} |");
			sb.AppendLine($"| {temp[9]} {temp[10]} {temp[11]} | {temp[12]} {temp[13]} {temp[14]} | {temp[15]} {temp[16]} {temp[17]} |");
			sb.AppendLine($"| {temp[18]} {temp[19]} {temp[20]} | {temp[21]} {temp[22]} {temp[23]} | {temp[24]} {temp[25]} {temp[26]} |");
			sb.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+");
			sb.AppendLine($"| {temp[27]} {temp[28]} {temp[29]} | {temp[30]} {temp[31]} {temp[32]} | {temp[33]} {temp[34]} {temp[35]} |");
			sb.AppendLine($"| {temp[36]} {temp[37]} {temp[38]} | {temp[39]} {temp[40]} {temp[41]} | {temp[42]} {temp[43]} {temp[44]} |");
			sb.AppendLine($"| {temp[45]} {temp[46]} {temp[47]} | {temp[48]} {temp[49]} {temp[50]} | {temp[51]} {temp[52]} {temp[53]} |");
			sb.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+");
			sb.AppendLine($"| {temp[54]} {temp[55]} {temp[56]} | {temp[57]} {temp[58]} {temp[59]} | {temp[60]} {temp[61]} {temp[62]} |");
			sb.AppendLine($"| {temp[63]} {temp[64]} {temp[65]} | {temp[66]} {temp[67]} {temp[68]} | {temp[69]} {temp[70]} {temp[71]} |");
			sb.AppendLine($"| {temp[72]} {temp[73]} {temp[74]} | {temp[75]} {temp[76]} {temp[77]} | {temp[78]} {temp[79]} {temp[80]} |");
			sb.AppendLine(SubtleGridLines ? "'-------+-------+-------'" : "+-------+-------+-------+");

			return sb.ToString();
		}

		/// <summary>
		/// To multiline string with candidates.
		/// </summary>
		/// <returns>The result.</returns>
		[SuppressMessage("", "IDE0004")]
		private string ToMultiLineStringCore()
		{
			#region Step 1
			// Step 1: gets the candidates information grouped by columns.
			var valuesByColumn = new Dictionary<int, IList<short>>
			{
				[0] = new List<short>(),
				[1] = new List<short>(),
				[2] = new List<short>(),
				[3] = new List<short>(),
				[4] = new List<short>(),
				[5] = new List<short>(),
				[6] = new List<short>(),
				[7] = new List<short>(),
				[8] = new List<short>()
			};
			var valuesByRow = new Dictionary<int, IList<short>>
			{
				[0] = new List<short>(),
				[1] = new List<short>(),
				[2] = new List<short>(),
				[3] = new List<short>(),
				[4] = new List<short>(),
				[5] = new List<short>(),
				[6] = new List<short>(),
				[7] = new List<short>(),
				[8] = new List<short>()
			};
			for (int i = 0; i < 81; i++)
			{
				short value = Grid.GetMask(i);
				valuesByRow[i / 9].Add(value);
				valuesByColumn[i % 9].Add(value);
			}
			#endregion

			#region Step 2
			// Step 2: gets the maximal number of candidates in a cell,
			// which is used for aligning by columns.
			int[] maxLengths = new int[9];
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
					int comparer = Math.Max(candidatesCount, GetCellStatus(value) switch
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
			#endregion

			#region Step 3
			// Step 3: outputs all characters.
			var sb = new StringBuilder();
			void PrintTabLines(char c1, char c2, char fillingChar)
			{
				sb.Append(c1);
				sb.Append(string.Empty.PadRight(maxLengths[0] + maxLengths[1] + maxLengths[2] + 6, fillingChar));
				sb.Append(c2);
				sb.Append(string.Empty.PadRight(maxLengths[3] + maxLengths[4] + maxLengths[5] + 6, fillingChar));
				sb.Append(c2);
				sb.Append(string.Empty.PadRight(maxLengths[6] + maxLengths[7] + maxLengths[8] + 6, fillingChar));
				sb.AppendLine(c1);
			}
			void PrintValues(IList<short> valuesByRow, int start, int end)
			{
				static string PrintCandidates(short value)
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

				sb.Append(" ");
				for (int i = start; i <= end; i++)
				{
					// Get digit.
					short value = valuesByRow[i];
					var cellStatus = GetCellStatus(value);
					int digit = cellStatus != CellStatus.Empty ? (~value).FindFirstSet() : -1;

					sb.Append(cellStatus switch
					{
						CellStatus.Given => $"<{digit + 1}>".PadRight(maxLengths[i]),
						CellStatus.Modifiable => TreatValueAsGiven switch
						{
							true => $"<{digit + 1}>".PadRight(maxLengths[i]),
							_ => $"*{digit + 1}*".PadRight(maxLengths[i])
						},
						_ => PrintCandidates(value).PadRight(maxLengths[i])
					});
					sb.Append(i != end ? "  " : " ");
				}
			}
			void PrintValueLines(IList<short> valuesByRow, char c1, char c2)
			{
				sb.Append(c1);
				PrintValues(valuesByRow, 0, 2);
				sb.Append(c2);
				PrintValues(valuesByRow, 3, 5);
				sb.Append(c2);
				PrintValues(valuesByRow, 6, 8);
				sb.AppendLine(c1);
			}

			for (int i = 0; i < 13; i++)
			{
				switch (i)
				{
					case 0: // Print tabs of the first line.
					{
						if (SubtleGridLines)
						{
							PrintTabLines('.', '.', '-');
						}
						else
						{
							PrintTabLines('+', '+', '-');
						}
						break;
					}
					case 4:
					case 8: // Print tabs of mediate lines.
					{
						if (SubtleGridLines)
						{
							PrintTabLines(':', '+', '-');
						}
						else
						{
							PrintTabLines('+', '+', '-');
						}
						break;
					}
					case 12: // Print tabs of the foot line.
					{
						if (SubtleGridLines)
						{
							PrintTabLines('\'', '\'', '-');
						}
						else
						{
							PrintTabLines('+', '+', '-');
						}
						break;
					}
					default: // Print values and tabs.
					{
						PrintValueLines(valuesByRow[i switch
						{
							_ when i >= 1 && i < 4 => i - 1,
							_ when i >= 5 && i < 8 => i - 2,
							_ when i >= 9 && i < 12 => i - 3,
							_ => throw new Exception("On the border.")
						}], '|', '|');
						break;
					}
				}
			}

			// The last step: returns the value.
			return sb.ToString();
			#endregion
		}

		/// <summary>
		/// Get cell status for a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The cell status.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static CellStatus GetCellStatus(short value) =>
			(CellStatus)(value >> 9 & (int)CellStatus.All);
	}
}
