namespace Sudoku.Text.Formatting;

/// <summary>
/// Provides a formatter that gathers the main information for a <see cref="Grid"/> instance,
/// and convert it to a <see cref="string"/> value as the result.
/// </summary>
public readonly ref struct GridFormatter
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
	public GridFormatter(bool multiline) :
		this(
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
	/// Indicates whether the formatter will process outline corner of the multi-line grid.
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
		_flags = placeholder switch { '0' => 1024, '.' => 0, _ => throw new ArgumentOutOfRangeException(nameof(placeholder)) };
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
		get => (_flags >> 10 & 1) == 0 ? '.' : '0';

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
	public string ToString(scoped in Grid grid)
		=> (Sukaku, Multiline, WithCandidates, Excel, HodokuCompatible, OpenSudoku) switch
		{
			(true, _, _, _, _, _) => ToSukakuString(grid),
			(_, true, true, _, _, _) => ToMultiLineStringCore(grid),
			(_, true, false, true, _, _) => ToExcelString(grid),
			(_, true, false, false, _, _) => ToMultiLineSimpleGridCore(grid),
			(_, false, _, _, true, _) => ToHodokuLibraryFormatString(grid),
			(_, false, _, _, false, true) => ToOpenSudokuString(grid),
			(_, false, _, _, false, false) => ToSingleLineStringCore(grid)
		};

	/// <summary>
	/// Represents a string value indicating this instance, with the specified format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="format">The string format.</param>
	/// <returns>The string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(scoped in Grid grid, string? format) => GridFormatterFactory.Create(format).ToString(grid);

	/// <summary>
	/// To Excel format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	private string ToExcelString(scoped in Grid grid)
	{
		scoped var span = grid.ToString("0").AsSpan();
		scoped var sb = new StringHandler(81 + 72 + 9);
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 9; j++)
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

		sb.RemoveFromEnd(Environment.NewLine.Length);
		return sb.ToStringAndClear();
	}

	/// <summary>
	/// To open sudoku format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	/// <exception cref="FormatException">Throws when the specified grid is invalid.</exception>
	private unsafe string ToOpenSudokuString(scoped in Grid grid)
	{
		// Calculates the length of the result string.
		const int length = 1 + (81 * 3 - 1 << 1);

		// Creates a string instance as a buffer.
		var result = new string('\0', length);

		// Modify the string value via pointers.
		fixed (char* pResult = result)
		{
			// Replace the base character with the separator.
			for (var pos = 1; pos < length; pos += 2)
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

		// Returns the result.
		return result;
	}

	/// <summary>
	/// To string with Hodoku library format compatible string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	private string ToHodokuLibraryFormatString(scoped in Grid grid)
		=> $":0000:x:{ToSingleLineStringCore(grid)}{new string(':', WithCandidates ? 2 : 3)}";

	/// <summary>
	/// To string with the sukaku format.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the puzzle is an invalid sukaku puzzle (at least one cell is given or modifiable).
	/// </exception>
	private string ToSukakuString(scoped in Grid grid)
	{
		if (Multiline)
		{
			// Append all digits.
			var builders = new StringBuilder[81];
			for (var i = 0; i < 81; i++)
			{
				builders[i] = new();
				foreach (var digit in grid.GetCandidates(i))
				{
					builders[i].Append(digit + 1);
				}
			}

			// Now consider the alignment for each column of output text.
			var sb = new StringBuilder();
			scoped var span = (stackalloc int[9]);
			for (var column = 0; column < 9; column++)
			{
				var maxLength = 0;
				for (var p = 0; p < 9; p++)
				{
					maxLength = Max(maxLength, builders[p * 9 + column].Length);
				}

				span[column] = maxLength;
			}
			for (var row = 0; row < 9; row++)
			{
				for (var column = 0; column < 9; column++)
				{
					var cell = row * 9 + column;
					sb.Append(builders[cell].ToString().PadLeft(span[column])).Append(' ');
				}
				sb.RemoveFrom(^1).AppendLine(); // Remove last whitespace.
			}

			return sb.ToString();
		}
		else
		{
			var sb = new StringBuilder();
			for (var i = 0; i < 81; i++)
			{
				sb.Append("123456789");
			}

			for (var i = 0; i < 729; i++)
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
	private string ToSingleLineStringCore(scoped in Grid grid)
	{
		scoped var sb = new StringHandler(162);
		var originalGrid = WithCandidates && !ShortenSusser ? Grid.Parse(grid.ToString(".+")) : Grid.Undefined;

		var eliminatedCandidates = Candidates.Empty;
		for (var c = 0; c < 81; c++)
		{
			var status = grid.GetStatus(c);
			if (status == CellStatus.Empty && !originalGrid.IsUndefined && WithCandidates)
			{
				// Check if the value has been set 'true'
				// and the value has already deleted at the grid
				// with only givens and modifiables.
				foreach (var i in (short)(originalGrid.GetMask(c) & Grid.MaxCandidatesMask))
				{
					if (!grid[c, i])
					{
						// The value is 'false', which means the digit has already been deleted.
						eliminatedCandidates.Add(c * 9 + i);
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

		var elimsStr = EliminationNotation.ToCandidatesString(eliminatedCandidates);
		var @base = sb.ToStringAndClear();
		return ShortenSusser ? shorten(@base, Placeholder) : $"{@base}{(string.IsNullOrEmpty(elimsStr) ? string.Empty : $":{elimsStr}")}";


		static unsafe string shorten(string @base, char placeholder)
		{
			scoped var resultSpan = (stackalloc char[81]);
			var index = 0;
			for (var i = 0; i < 9; i++)
			{
				var sliced = @base.Substring(i * 9, 9);
				var collection = Regex.Matches(sliced, $"{(placeholder == '.' ? @"\." : "0")}+");
				if (collection.Count == 0)
				{
					// Can't find any simplifications.
					fixed (char* p = resultSpan.Slice(i * 9, 9), q = sliced)
					{
						CopyBlock(p, q, sizeof(char) * 9);
					}

					index += 9;
				}
				else
				{
					var set = new HashSet<Match>(collection, new MatchLengthComparer());
					if (set.Count == 1)
					{
						// All matches are same-length.
						var j = 0;
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
						var match = set.MaxBy(static m => m.Length)!.Value;
						var pos = sliced.IndexOf(match);
						var j = 0;
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
	private unsafe string ToMultiLineStringCore(scoped in Grid grid)
	{
		// Step 1: gets the candidates information grouped by columns.
		Dictionary<int, List<short>> valuesByColumn = new()
		{
			{ 0, new() },
			{ 1, new() },
			{ 2, new() },
			{ 3, new() },
			{ 4, new() },
			{ 5, new() },
			{ 6, new() },
			{ 7, new() },
			{ 8, new() }
		}, valuesByRow = new()
		{
			{ 0, new() },
			{ 1, new() },
			{ 2, new() },
			{ 3, new() },
			{ 4, new() },
			{ 5, new() },
			{ 6, new() },
			{ 7, new() },
			{ 8, new() }
		};

		for (var i = 0; i < 81; i++)
		{
			var value = grid.GetMask(i);
			valuesByRow[i / 9].Add(value);
			valuesByColumn[i % 9].Add(value);
		}

		// Step 2: gets the maximal number of candidates in a cell,
		// which is used for aligning by columns.
		const int bufferLength = 9;
		var maxLengths = stackalloc int[bufferLength];
		InitBlock(maxLengths, 0, sizeof(int) * bufferLength);

		foreach (var (i, _) in valuesByColumn)
		{
			var maxLength = maxLengths + i;

			// Iteration on row index.
			for (var j = 0; j < 9; j++)
			{
				// Gets the number of candidates.
				var candidatesCount = 0;
				var value = valuesByColumn[i][j];

				// Iteration on each candidate.
				// Counts the number of candidates.
				candidatesCount += PopCount((uint)value);

				// Compares the values.
				var comparer = Max(
					candidatesCount,
					MaskToStatus(value) switch
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
		scoped var sb = new StringHandler();
		for (var i = 0; i < 13; i++)
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
						scoped in GridFormatter formatter, scoped ref StringHandler sb,
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
							scoped in GridFormatter formatter, scoped ref StringHandler sb,
							IList<short> valuesByRow, int start, int end, int* maxLengths)
						{
							sb.Append(' ');
							for (var i = start; i <= end; i++)
							{
								// Get digit.
								var value = valuesByRow[i];
								var status = MaskToStatus(value);

								value &= Grid.MaxCandidatesMask;
								var d = value == 0 ? -1 : (status != CellStatus.Empty ? TrailingZeroCount(value) : -1) + 1;
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
										var innerSb = new StringHandler(9);
										foreach (var z in value)
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
		sb.RemoveFromEnd(Environment.NewLine.Length);
		return sb.ToString();


		static void printTabLines(scoped ref StringHandler sb, char c1, char c2, char fillingChar, int* m)
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

	/// <summary>
	/// To multi-line normal grid string without any candidates.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The result.</returns>
	private string ToMultiLineSimpleGridCore(scoped in Grid grid)
	{
		var t = grid.ToString(TreatValueAsGiven ? $"{Placeholder}!" : Placeholder.ToString());
		return new StringBuilder()
			.AppendLine(SubtleGridLines ? ".-------.-------.-------." : "+-------+-------+-------+")
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
			.Append(SubtleGridLines ? "'-------'-------'-------'" : "+-------+-------+-------+")
			.ToString();
	}


	/// <summary>
	/// Create a <see cref="GridFormatter"/> according to the specified grid output options.
	/// </summary>
	/// <param name="gridOutputOption">The grid output options.</param>
	/// <returns>The grid formatter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GridFormatter Create(GridFormattingOptions gridOutputOption)
		=> gridOutputOption switch
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
}

/// <summary>
/// Represents a comparer instance that compares two <see cref="Match"/> instances via their length.
/// </summary>
file sealed class MatchLengthComparer : IEqualityComparer<Match>
{
	/// <inheritdoc/>
	public bool Equals(Match? x, Match? y) => (x?.Value.Length ?? -1) == (y?.Value.Length ?? -1);

	/// <inheritdoc/>
	public int GetHashCode([DisallowNull] Match? obj) => obj?.Value.Length ?? -1;
}
