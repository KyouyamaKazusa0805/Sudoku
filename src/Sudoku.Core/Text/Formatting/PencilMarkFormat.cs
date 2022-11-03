namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents with a pencil-marked formatter.
/// </summary>
/// <param name="SubtleGridLines">
/// <para>Indicates whether need to handle all grid outlines while outputting.</para>
/// <para>The default value is <see langword="true"/>.</para>
/// </param>
/// <param name="TreatValueAsGiven">
/// <para>
/// Indicates the output will treat modifiable values as given ones.
/// If the output is single line, the output will remove all plus marks '+'.
/// If the output is multi-line, the output will use '<c><![CDATA[<digit>]]></c>' instead
/// of '<c>*digit*</c>'.
/// </para>
/// <para>The default value is <see langword="false"/>.</para>
/// </param>
public sealed record PencilMarkFormat(bool SubtleGridLines = true, bool TreatValueAsGiven = false) : IGridFormatter
{
	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="SubtleGridLines"/>: <see langword="true"/></item>
	/// <item><see cref="TreatValueAsGiven"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly PencilMarkFormat Default = new();


	/// <inheritdoc/>
	static IGridFormatter IGridFormatter.Instance => Default;


	/// <inheritdoc/>
	public unsafe string ToString(scoped in Grid grid)
	{
		// Step 1: gets the candidates information grouped by columns.
		var valuesByColumn = CreateTempDictionary();
		var valuesByRow = CreateTempDictionary();

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
						CellStatus.Given => Max(candidatesCount, 3), // The output will be '<digit>' and consist of 3 characters.
						CellStatus.Modifiable => Max(candidatesCount, 3), // The output will be '*digit*' and consist of 3 characters.
						_ => candidatesCount, // Normal output: 'series' (at least 1 character).
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
						PrintTabLines(ref sb, '.', '.', '-', maxLengths);
					}
					else
					{
						PrintTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				case 4 or 8: // Print tabs of mediate lines.
				{
					if (SubtleGridLines)
					{
						PrintTabLines(ref sb, ':', '+', '-', maxLengths);
					}
					else
					{
						PrintTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				case 12: // Print tabs of the foot line.
				{
					if (SubtleGridLines)
					{
						PrintTabLines(ref sb, '\'', '\'', '-', maxLengths);
					}
					else
					{
						PrintTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				default: // Print values and tabs.
				{
					DefaultPrinting(ref sb, valuesByRow[A057353(i)], '|', '|', maxLengths);

					break;
				}
			}
		}

		// The last step: returns the value.
		return sb.ToString();
	}

	/// <summary>
	/// Default printing method.
	/// </summary>
	private unsafe void DefaultPrinting(scoped ref StringHandler sb, IList<short> valuesByRow, char c1, char c2, int* maxLengths)
	{
		sb.Append(c1);
		PrintValues(ref sb, valuesByRow, 0, 2, maxLengths);
		sb.Append(c2);
		PrintValues(ref sb, valuesByRow, 3, 5, maxLengths);
		sb.Append(c2);
		PrintValues(ref sb, valuesByRow, 6, 8, maxLengths);
		sb.Append(c1);
		sb.AppendLine();
	}

	/// <summary>
	/// Print values.
	/// </summary>
	private unsafe void PrintValues(scoped ref StringHandler sb, IList<short> valuesByRow, int start, int end, int* maxLengths)
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
				case CellStatus.Modifiable when TreatValueAsGiven:
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
					scoped var innerSb = new StringHandler(9);
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

	/// <summary>
	/// Print tab lines.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void PrintTabLines(scoped ref StringHandler sb, char c1, char c2, char fillingChar, int* m)
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

	/// <summary>
	/// Creates a dictionary.
	/// </summary>
	/// <returns>The dictionary instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static IDictionary<int, List<short>> CreateTempDictionary()
		=> new Dictionary<int, List<short>>
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
}
