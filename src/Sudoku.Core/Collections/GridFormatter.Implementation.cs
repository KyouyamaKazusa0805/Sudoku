using Sudoku.Diagnostics.CodeAnalysis;
using static System.Algorithm.Sequences;
using static System.Math;
using static System.Numerics.BitOperations;

namespace Sudoku.Collections;

partial struct GridFormatter
{
	/// <summary>
	/// To Excel format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	private partial string ToExcelString(in Grid grid)
	{
		ReadOnlySpan<char> span = grid.ToString("0");
		var sb = new StringHandler(initialCapacity: 81 + 72 + 9);
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (span[i * 9 + j] - '0' is var digit and not 0)
				{
					sb.Append(digit);
				}

				sb.Append('\t');
			}

			sb.RemoveFromEnd(1);
			sb.AppendLine();
		}

		return sb.ToStringAndClear();
	}

	/// <summary>
	/// To open sudoku format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	/// <exception cref="FormatException">Throws when the specified grid is invalid.</exception>
	private partial string ToOpenSudokuString(in Grid grid)
	{
		// Calculates the length of the result string.
		const int length = 1 + (81 * 3 - 1 << 1);

		// Creates a string instance as a buffer.
		string result = new('\0', length);

		unsafe
		{
			// Modify the string value via pointers.
			fixed (char* pResult = result)
			{
				// Replace the base character with the separator.
				for (int pos = 1; pos < length; pos += 2)
				{
					pResult[pos] = '|';
				}

				// Now replace some positions with the specified values.
				for (int i = 0, pos = 0; i < 81; i++, pos += 6)
				{
					switch (grid.GetStatus(i))
					{
						case CellStatus.Empty:
						{
							pResult[pos] = '0';
							pResult[pos + 2] = '0';
							pResult[pos + 4] = '1';

							break;
						}
						case CellStatus.Modifiable:
						case CellStatus.Given:
						{
							pResult[pos] = (char)(grid[i] + '1');
							pResult[pos + 2] = '0';
							pResult[pos + 4] = '0';

							break;
						}
						default:
						{
							throw new FormatException("The specified grid is invalid.");
						}
					}
				}
			}
		}

		// Returns the result.
		return result;
	}

	/// <summary>
	/// To string with Hodoku library format compatible string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	private partial string ToHodokuLibraryFormatString(in Grid grid) =>
		$":0000:x:{ToSingleLineStringCore(grid)}:::";

	/// <summary>
	/// To string with the sukaku format.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the puzzle is an invalid sukaku puzzle (at least one cell is given or modifiable).
	/// </exception>
	private partial string ToSukakuString(in Grid grid)
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
					sb.Append(builders[cell].ToString().PadLeft(span[column])).Append(' ');
				}
				sb.RemoveFrom(^1).AppendLine(); // Remove last whitespace.
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
				if (!grid[i / 9, i % 9])
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
	private partial string ToSingleLineStringCore(in Grid grid)
	{
		var sb = new StringHandler(initialCapacity: 162);
		var elims = new StringHandler();
		var originalGrid = WithCandidates && !ShortenSusser ? Grid.Parse(grid.ToString(".+")) : Grid.Undefined;

		unsafe
		{
			for (int c = 0; c < 81; c++)
			{
				var status = grid.GetStatus(c);
				if (status == CellStatus.Empty && !originalGrid.IsUndefined && WithCandidates)
				{
					// Check if the value has been set 'true'
					// and the value has already deleted at the grid
					// with only givens and modifiables.
					foreach (int i in originalGrid.GetMask(c) & Grid.MaxCandidatesMask)
					{
						if (!grid[c, i])
						{
							// The value is 'true', which means the digit has already been deleted.
							elims.Append(i + 1);
							elims.Append(c / 9 + 1);
							elims.Append(c % 9 + 1);
							elims.Append(' ');
						}
					}
				}

				switch (status)
				{
					case CellStatus.Empty:
					{
						sb.Append(Placeholder);
						break;
					}
					case CellStatus.Modifiable:
					{
						if (WithModifiables && !ShortenSusser)
						{
							sb.Append('+');
							sb.Append(grid[c] + 1);
						}
						else
						{
							sb.Append(Placeholder);
						}

						break;
					}
					case CellStatus.Given:
					{
						sb.Append(grid[c] + 1);
						break;
					}
					default:
					{
						throw new InvalidOperationException("The specified status is invalid.");
					}
				}
			}
		}

		string elimsStr;
		if (elims.Length <= 3)
		{
			elimsStr = elims.ToStringAndClear();
		}
		else
		{
			elims.RemoveFromEnd(1);
			elimsStr = elims.ToStringAndClear();
		}

		string @base = sb.ToStringAndClear();
		return ShortenSusser
			? shorten(@base, Placeholder)
			: $"{@base}{(string.IsNullOrEmpty(elimsStr) ? string.Empty : $":{elimsStr}")}";


		static unsafe string shorten(string @base, char placeholder)
		{
			var resultSpan = (stackalloc char[81]);
			int index = 0;
			for (int i = 0; i < 9; i++)
			{
				string sliced = @base.Substring(i * 9, 9);
				var collection = Regex.Matches(sliced, $"{(placeholder == '.' ? @"\." : "0")}+");
				if (collection.Count == 0)
				{
					// Can't find any simplifications.
					fixed (char* p = resultSpan.Slice(i * 9, 9), q = sliced)
					{
						Unsafe.CopyBlock(p, q, sizeof(char) * 9);
					}

					index += 9;
				}
				else
				{
					var set = new HashSet<Match>(collection, new MatchLengthComparer());
					if (set.Count == 1)
					{
						// All matches are same-length.
						int j = 0;
						while (j < 9)
						{
							if (sliced[j] == placeholder)
							{
								resultSpan[index++] = '*';
								j += set.First().Length;
							}
							else
							{
								resultSpan[index++] = sliced[j];
								j++;
							}
						}
					}
					else
					{
						string match = set.MaxBy(static m => m.Length)!.Value;
						int pos = sliced.IndexOf(match);
						int j = 0;
						while (j < 9)
						{
							if (j == pos)
							{
								resultSpan[index++] = '*';
								j += match.Length;
							}
							else
							{
								resultSpan[index++] = sliced[j];
								j++;
							}
						}
					}
				}

				if (i != 8)
				{
					resultSpan[index++] = ',';
				}
			}

			return resultSpan[..index].ToString();
		}
	}

	/// <summary>
	/// To multi-line string with candidates.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The result.</returns>
	private partial string ToMultiLineStringCore(in Grid grid)
	{
		// Step 1: gets the candidates information grouped by columns.
		Dictionary<int, IList<short>> valuesByColumn = new()
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
		}, valuesByRow = new()
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

		for (int i = 0; i < 81; i++)
		{
			short value = grid.GetMask(i);
			valuesByRow[i / 9].Add(value);
			valuesByColumn[i % 9].Add(value);
		}

		unsafe
		{
			// Step 2: gets the maximal number of candidates in a cell,
			// which is used for aligning by columns.
			const int bufferLength = 9;
			int* maxLengths = stackalloc int[bufferLength];
			Unsafe.InitBlock(maxLengths, 0, sizeof(int) * bufferLength);

			foreach (var (i, _) in valuesByColumn)
			{
				int* maxLength = maxLengths + i;

				// Iteration on row index.
				for (int j = 0; j < 9; j++)
				{
					// Gets the number of candidates.
					int candidatesCount = 0;
					short value = valuesByColumn[i][j];

					// Iteration on each candidate.
					// Counts the number of candidates.
					candidatesCount += PopCount((uint)value);

					// Compares the values.
					int comparer = Max(
						candidatesCount,
						Grid.MaskToStatus(value) switch
						{
							// The output will be '<digit>' and consist of 3 characters.
							CellStatus.Given => Max(candidatesCount, 3),
							// The output will be '*digit*' and consist of 3 characters.
							CellStatus.Modifiable => Max(candidatesCount, 3),
							// Normal output: 'series' (at least 1 character).
							_ => candidatesCount,
						}
					);
					if (comparer > *maxLength)
					{
						*maxLength = comparer;
					}
				}
			}

			// Step 3: outputs all characters.
			var sb = new StringHandler();
			for (int i = 0; i < 13; i++)
			{
				switch (i)
				{
					case 0: // Print tabs of the first line.
					{
						if (SubtleGridLines)
						{
							printTabLines(ref sb, '.', '.', '-', maxLengths);
						}
						else
						{
							printTabLines(ref sb, '+', '+', '-', maxLengths);
						}
						break;
					}
					case 4:
					case 8: // Print tabs of mediate lines.
					{
						if (SubtleGridLines)
						{
							printTabLines(ref sb, ':', '+', '-', maxLengths);
						}
						else
						{
							printTabLines(ref sb, '+', '+', '-', maxLengths);
						}
						break;
					}
					case 12: // Print tabs of the foot line.
					{
						if (SubtleGridLines)
						{
							printTabLines(ref sb, '\'', '\'', '-', maxLengths);
						}
						else
						{
							printTabLines(ref sb, '+', '+', '-', maxLengths);
						}
						break;
					}
					default: // Print values and tabs.
					{
						p(this, ref sb, valuesByRow[A057353(i)], '|', '|', maxLengths);

						break;


						static void p(
							in GridFormatter formatter, ref StringHandler sb,
							IList<short> valuesByRow, char c1, char c2, int* maxLengths)
						{
							sb.Append(c1);
							printValues(formatter, ref sb, valuesByRow, 0, 2, maxLengths);
							sb.Append(c2);
							printValues(formatter, ref sb, valuesByRow, 3, 5, maxLengths);
							sb.Append(c2);
							printValues(formatter, ref sb, valuesByRow, 6, 8, maxLengths);
							sb.Append(c1);
							sb.AppendLine();


							static void printValues(
								in GridFormatter formatter, ref StringHandler sb,
								IList<short> valuesByRow, int start, int end, int* maxLengths)
							{
								sb.Append(' ');
								for (int i = start; i <= end; i++)
								{
									// Get digit.
									short value = valuesByRow[i];
									var status = Grid.MaskToStatus(value);

									value &= Grid.MaxCandidatesMask;
									int d = value == 0
										? -1
										: (status != CellStatus.Empty ? TrailingZeroCount(value) : -1) + 1;
									string s;
									switch (status)
									{
										case CellStatus.Given:
										case CellStatus.Modifiable when formatter.TreatValueAsGiven:
										{
											s = $"<{d}>";
											break;
										}
										case CellStatus.Modifiable:
										{
											s = $"*{d}*";
											break;
										}
										default:
										{
											var innerSb = new StringHandler(initialCapacity: 9);
											foreach (int z in value)
											{
												innerSb.Append(z + 1);
											}

											s = innerSb.ToStringAndClear();

											break;
										}
									}

									sb.Append(s.PadRight(maxLengths[i]));
									sb.Append(i != end ? "  " : " ");
								}
							}
						}
					}
				}
			}

			// The last step: returns the value.
			return sb.ToString();


			static void printTabLines(ref StringHandler sb, char c1, char c2, char fillingChar, int* m)
			{
				sb.Append(c1);
				sb.Append(string.Empty.PadRight(m[0] + m[1] + m[2] + 6, fillingChar));
				sb.Append(c2);
				sb.Append(string.Empty.PadRight(m[3] + m[4] + m[5] + 6, fillingChar));
				sb.Append(c2);
				sb.Append(string.Empty.PadRight(m[6] + m[7] + m[8] + 6, fillingChar));
				sb.Append(c1);
				sb.AppendLine();
			}
		}
	}

	/// <summary>
	/// To multi-line normal grid string without any candidates.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The result.</returns>
	private partial string ToMultiLineSimpleGridCore(in Grid grid)
	{
		string t = grid.ToString(TreatValueAsGiven ? $"{Placeholder}!" : Placeholder.ToString());
		return new StringBuilder()
			.AppendLine(SubtleGridLines ? ".-------+-------+-------." : "+-------+-------+-------+")
			.Append("| ").Append(t[0]).Append(' ').Append(t[1]).Append(' ').Append(t[2])
			.Append(" | ").Append(t[3]).Append(' ').Append(t[4]).Append(' ').Append(t[5])
			.Append(" | ").Append(t[6]).Append(' ').Append(t[7]).Append(' ').Append(t[8])
			.AppendLine(" |")
			.Append("| ").Append(t[9]).Append(' ').Append(t[10]).Append(' ').Append(t[11])
			.Append(" | ").Append(t[12]).Append(' ').Append(t[13]).Append(' ').Append(t[14])
			.Append(" | ").Append(t[15]).Append(' ').Append(t[16]).Append(' ').Append(t[17])
			.AppendLine(" |")
			.Append("| ").Append(t[18]).Append(' ').Append(t[19]).Append(' ').Append(t[20])
			.Append(" | ").Append(t[21]).Append(' ').Append(t[22]).Append(' ').Append(t[23])
			.Append(" | ").Append(t[24]).Append(' ').Append(t[25]).Append(' ').Append(t[26])
			.AppendLine(" |")
			.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+")
			.Append("| ").Append(t[27]).Append(' ').Append(t[28]).Append(' ').Append(t[29])
			.Append(" | ").Append(t[30]).Append(' ').Append(t[31]).Append(' ').Append(t[32])
			.Append(" | ").Append(t[33]).Append(' ').Append(t[34]).Append(' ').Append(t[35])
			.AppendLine(" |")
			.Append("| ").Append(t[36]).Append(' ').Append(t[37]).Append(' ').Append(t[38])
			.Append(" | ").Append(t[39]).Append(' ').Append(t[40]).Append(' ').Append(t[41])
			.Append(" | ").Append(t[42]).Append(' ').Append(t[43]).Append(' ').Append(t[44])
			.AppendLine(" |")
			.Append("| ").Append(t[45]).Append(' ').Append(t[46]).Append(' ').Append(t[47])
			.Append(" | ").Append(t[48]).Append(' ').Append(t[49]).Append(' ').Append(t[50])
			.Append(" | ").Append(t[51]).Append(' ').Append(t[52]).Append(' ').Append(t[53])
			.AppendLine(" |")
			.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+")
			.Append("| ").Append(t[54]).Append(' ').Append(t[55]).Append(' ').Append(t[56])
			.Append(" | ").Append(t[57]).Append(' ').Append(t[58]).Append(' ').Append(t[59])
			.Append(" | ").Append(t[60]).Append(' ').Append(t[61]).Append(' ').Append(t[62])
			.AppendLine(" |")
			.Append("| ").Append(t[63]).Append(' ').Append(t[64]).Append(' ').Append(t[65])
			.Append(" | ").Append(t[66]).Append(' ').Append(t[67]).Append(' ').Append(t[68])
			.Append(" | ").Append(t[69]).Append(' ').Append(t[70]).Append(' ').Append(t[71])
			.AppendLine(" |")
			.Append("| ").Append(t[72]).Append(' ').Append(t[73]).Append(' ').Append(t[74])
			.Append(" | ").Append(t[75]).Append(' ').Append(t[76]).Append(' ').Append(t[77])
			.Append(" | ").Append(t[78]).Append(' ').Append(t[79]).Append(' ').Append(t[80])
			.AppendLine(" |")
			.AppendLine(SubtleGridLines ? "'-------+-------+-------'" : "+-------+-------+-------+")
			.ToString();
	}


	/// <summary>
	/// Indicates the inner equality comparer to determine the equality of length
	/// of 2 <see cref="Match"/>es to compare.
	/// </summary>
	[AnonymousInnerType]
	private sealed class MatchLengthComparer : IEqualityComparer<Match>
	{
		public bool Equals(Match? x, Match? y) => (x?.Value.Length ?? -1) == (y?.Value.Length ?? -1);

		public int GetHashCode(Match? obj) => obj?.Value.Length ?? -1;
	}
}