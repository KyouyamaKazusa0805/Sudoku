namespace Sudoku.Text.Notations;

/// <summary>
/// Encapsulates a set of methods that handles a variety of instances, using K9 notation
/// to output the <see cref="string"/> representation, or parse a <see cref="string"/> value to convert
/// it to the suitable-typed instance.
/// </summary>
/// <remarks>
/// <para>
/// The <b>K9 notation</b> is a notation to describe a set of cells that uses letters
/// A, B, C, D, E, F, G, H and K to describe the row, and uses digits 1 to 9 to describe the column.
/// For example, <c>C8</c> means the cell at row 3 and column 8.
/// The letter I and J aren't used in this notation because they are confusing with digit 1.
/// However, they can also be used in Chinese notations, for example, <c>K8</c> in traditional notation
/// is same as <c>I8</c> in Chinese K9 notation rule.
/// </para>
/// <para>
/// For more information about this concept, please visit
/// <see href="http://sudopedia.enjoysudoku.com/K9.html">this link</see>.
/// </para>
/// <para>
/// Please note that the type is an <see langword="abstract"/> type,
/// which means you cannot instantiate any objects. In addition, the type contains
/// a <see langword="private"/> instance constructor, which disallows you deriving any types.
/// </para>
/// </remarks>
public sealed partial class K9Notation : ICellNotation<K9Notation, K9NotationOptions>
{
	/// <summary>
	/// Indicates all possible letters that used in the row notation.
	/// </summary>
	private static readonly char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K' };


	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, false, DiagnosticId = "SCA0108", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0108")]
	private K9Notation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static CellNotation CellNotation => CellNotation.K9;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCellsString(scoped in CellMap cells) => ToCellsString(cells, K9NotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCellsString(scoped in CellMap cells, scoped in K9NotationOptions options)
	{
		return cells switch
		{
			[] => string.Empty,
			[var p] => options switch
			{
				{ AvoidConfusionOnRowLetters: true, UpperCasing: var isUpper }
					=> $"{(isUpper ? Letters[p / 9] : char.ToLower(Letters[p / 9]))}{p % 9 + 1}",
				{ UpperCasing: var isUpper }
					=> $"{(isUpper ? (char)('A' + p / 9) : char.ToLower((char)('A' + p / 9)))}{p % 9 + 1}"
			},
			_ => r(cells, options) is var a && c(cells, options) is var b && a.Length <= b.Length ? a : b
		};


		static string i(int v) => (v + 1).ToString();

		static unsafe string r(scoped in CellMap cells, scoped in K9NotationOptions options)
		{
			scoped var sbRow = new StringHandler(18);
			var dic = new Dictionary<int, List<int>>(9);
			var (upperCasing, avoidConfusionOnRowLetters, separator) = options;
			foreach (var cell in cells)
			{
				if (!dic.ContainsKey(cell / 9))
				{
					dic.Add(cell / 9, new(9));
				}

				dic[cell / 9].Add(cell % 9);
			}
			foreach (var row in dic.Keys)
			{
				if (avoidConfusionOnRowLetters)
				{
					sbRow.Append(upperCasing ? Letters[row] : char.ToLower(Letters[row]));
				}
				else
				{
					sbRow.Append((upperCasing ? 'A' : 'a') + row);
				}
				sbRow.AppendRange(dic[row], &i);
				sbRow.Append(separator);
			}
			sbRow.RemoveFromEnd(1);

			return sbRow.ToStringAndClear();
		}

		static unsafe string c(scoped in CellMap cells, scoped in K9NotationOptions options)
		{
			var dic = new Dictionary<int, List<int>>(9);
			scoped var sbColumn = new StringHandler(18);
			var (upperCasing, avoidConfusionOnRowLetters, separator) = options;
			foreach (var cell in cells)
			{
				if (!dic.ContainsKey(cell % 9))
				{
					dic.Add(cell % 9, new(9));
				}

				dic[cell % 9].Add(cell / 9);
			}

			foreach (var column in dic.Keys)
			{
				foreach (var row in dic[column])
				{
					if (avoidConfusionOnRowLetters)
					{
						sbColumn.Append(upperCasing ? Letters[row] : char.ToLower(Letters[row]));
					}
					else
					{
						sbColumn.Append((upperCasing ? 'A' : 'a') + row);
					}
				}

				sbColumn.Append(column + 1);
				sbColumn.Append(separator);
			}
			sbColumn.RemoveFromEnd(1);

			return sbColumn.ToStringAndClear();
		}
	}

	/// <summary>
	/// Try to parse the string and convert it into a single cell index.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The valid cell index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ParseCell(string str) => ParseCells(str)[0];

	/// <inheritdoc/>
	public static unsafe CellMap ParseCells(string str)
	{
		// Check whether the match is successful.
		if (CellOrCellListRegex().Matches(str) is not { Count: not 0 } matches)
		{
			throw new FormatException("The specified string can't match any cell instance.");
		}

		// Declare the buffer.
		int* bufferRows = stackalloc int[9], bufferColumns = stackalloc int[9];

		// Declare the result variable.
		var result = CellMap.Empty;

		// Iterate on each match instance.
		foreach (var match in matches.Cast<Match>())
		{
			var value = match.Value;
			char* anchorR, anchorC;
			fixed (char* pValue = value)
			{
				anchorR = anchorC = pValue;

				while (!char.IsDigit(*anchorC))
				{
					anchorC++;
				}
			}

			// Stores the possible values into the buffer.
			int rIndex = 0, cIndex = 0;
			for (var p = anchorR; !char.IsDigit(*p); p++, rIndex++)
			{
				bufferRows[rIndex] = *p switch
				{
					'K' or 'k' or 'I' or 'i' => 8,
					>= 'A' and <= 'H' => *p - 'A',
					_ => *p - 'a'
				};
			}
			for (var p = anchorC; *p != '\0'; p++, cIndex++)
			{
				bufferColumns[cIndex] = *p - '1';
			}

			// Now combine two buffers.
			for (var i = 0; i < rIndex; i++)
			{
				for (var j = 0; j < cIndex; j++)
				{
					result.Add(bufferRows[i] * 9 + bufferColumns[j]);
				}
			}
		}

		// Returns the result.
		return result;
	}

	/// <inheritdoc cref="RxCyNotation.TryParseCell(string, out int)"/>
	public static bool TryParseCell(string str, out int cell)
	{
		try
		{
			cell = ParseCell(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out cell);
			return false;
		}
	}

	/// <inheritdoc/>
	public static bool TryParseCells(string str, out CellMap result)
	{
		try
		{
			result = ParseCells(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out result);
			return false;
		}
	}

	/// <summary>
	/// Indicates the regular expression for matching a cell or cell-list.
	/// </summary>
	[GeneratedRegex("""[A-IKa-ik]{1,9}[1-9]{1,9}""", RegexOptions.Compiled, 5000)]
	private static partial Regex CellOrCellListRegex();
}
