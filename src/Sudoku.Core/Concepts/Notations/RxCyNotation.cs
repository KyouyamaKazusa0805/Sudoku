namespace Sudoku.Concepts.Notations;

/// <summary>
/// Encapsulates a set of methods that handles a variety of instances, using RxCy notation
/// to output the <see cref="string"/> representation, or parse a <see cref="string"/> value to convert
/// it to the suitable-typed instance.
/// </summary>
/// <remarks>
/// <para>
/// The <b>RxCy notation</b> is a notation to describe a set of cells that uses letter
/// <c>R</c> (or its lower case <c>r</c>) to describe a row label, and uses the other letter
/// <c>C</c> (or its lower case <c>c</c>) to describe a column label. For example,
/// <c>R4C2</c> means the cell at row 4 and column 2.
/// </para>
/// <para>
/// For more information about this concept, please visit
/// <see href="http://sudopedia.enjoysudoku.com/Rncn.html">this link</see>.
/// </para>
/// <para>
/// Please note that the type is an <see langword="abstract"/> type,
/// which means you cannot instantiate any objects. In addition, the type contains
/// a <see langword="private"/> instance constructor, which disallows you deriving any types.
/// </para>
/// </remarks>
public sealed class RxCyNotation : INotationHandler, ICellNotation<RxCyNotation, RxCyNotationOptions>
{
	/// <summary>
	/// Indicates the regular expression for matching a cell or cell-list.
	/// </summary>
	private static readonly Regex CellOrCellListRegex = new(
		"""(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})""",
		RegexOptions.ExplicitCapture,
		TimeSpan.FromSeconds(5)
	);


	[Obsolete("Please don't call this constructor.", true)]
	private RxCyNotation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public Notation Notation => Notation.RxCy;


	/// <inheritdoc/>
	public static bool TryParseCells(string str, out Cells result)
	{
		try
		{
			result = ParseCells(str);
			return true;
		}
		catch (FormatException)
		{
			Unsafe.SkipInit(out result);
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCellsString(in Cells cells) => ToCellsString(cells, RxCyNotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCellsString(in Cells cells, in RxCyNotationOptions options)
	{
		bool upperCasing = options.UpperCasing;
		return cells switch
		{
			[] => string.Empty,
			[var p] => $"{rowLabel(upperCasing)}{p / 9 + 1}{columnLabel(upperCasing)}{p % 9 + 1}",
			_ => r(cells, options) is var a && c(cells, options) is var b && a.Length <= b.Length ? a : b
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static char rowLabel(bool upperCasing) => upperCasing ? 'R' : 'r';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static char columnLabel(bool upperCasing) => upperCasing ? 'C' : 'c';

		static string i(int v) => (v + 1).ToString();

		static unsafe string r(in Cells cells, in RxCyNotationOptions options)
		{
			var sbRow = new StringHandler(50);
			var dic = new Dictionary<int, List<int>>(9);
			var (upperCasing, separator) = options;
			foreach (int cell in cells)
			{
				if (!dic.ContainsKey(cell / 9))
				{
					dic.Add(cell / 9, new(9));
				}

				dic[cell / 9].Add(cell % 9);
			}
			foreach (int row in dic.Keys)
			{
				sbRow.Append(rowLabel(upperCasing));
				sbRow.Append(row + 1);
				sbRow.Append(columnLabel(upperCasing));
				sbRow.AppendRange(dic[row], &i);
				sbRow.Append(separator);
			}
			sbRow.RemoveFromEnd(1);

			return sbRow.ToStringAndClear();
		}

		static unsafe string c(in Cells cells, in RxCyNotationOptions options)
		{
			var dic = new Dictionary<int, List<int>>(9);
			var sbColumn = new StringHandler(50);
			var (upperCasing, separator) = options;
			foreach (int cell in cells)
			{
				if (!dic.ContainsKey(cell % 9))
				{
					dic.Add(cell % 9, new(9));
				}

				dic[cell % 9].Add(cell / 9);
			}

			foreach (int column in dic.Keys)
			{
				sbColumn.Append(rowLabel(upperCasing));
				sbColumn.AppendRange(dic[column], &i);
				sbColumn.Append(columnLabel(upperCasing));
				sbColumn.Append(column + 1);
				sbColumn.Append(separator);
			}
			sbColumn.RemoveFromEnd(1);

			return sbColumn.ToStringAndClear();
		}
	}

	/// <inheritdoc/>
	public static unsafe Cells ParseCells(string str)
	{
		// Check whether the match is successful.
		if (CellOrCellListRegex.Matches(str) is not [_, ..] matches)
		{
			throw new FormatException("The specified string can't match any cell instance.");
		}

		// Declare the buffer.
		Span<int> bufferRows = stackalloc int[9], bufferColumns = stackalloc int[9];

		// Declare the result variable.
		var result = Cells.Empty;

		// Iterate on each match instance.
		foreach (Match match in matches)
		{
			string value = match.Value;
			char* anchorR, anchorC;
			fixed (char* pValue = value)
			{
				anchorR = anchorC = pValue;

				// Find the index of the character 'C'.
				// The regular expression guaranteed the string must contain the character 'C' or 'c',
				// so we don't need to check '*p != '\0''.
				while (*anchorC is not ('C' or 'c'/* or '\0'*/))
				{
					anchorC++;
				}
			}

			// Stores the possible values into the buffer.
			int rIndex = 0, cIndex = 0;
			for (char* p = anchorR + 1; *p is not ('C' or 'c'); p++, rIndex++)
			{
				bufferRows[rIndex] = *p - '1';
			}
			for (char* p = anchorC + 1; *p != '\0'; p++, cIndex++)
			{
				bufferColumns[cIndex] = *p - '1';
			}

			// Now combine two buffers.
			for (int i = 0; i < rIndex; i++)
			{
				for (int j = 0; j < cIndex; j++)
				{
					result.Add(bufferRows[i] * 9 + bufferColumns[j]);
				}
			}
		}

		// Returns the result.
		return result;
	}
}
