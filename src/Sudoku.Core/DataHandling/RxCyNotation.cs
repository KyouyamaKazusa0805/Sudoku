using Sudoku.Collections;
using static Sudoku.Constants;

namespace Sudoku.DataHandling;

/// <summary>
/// Defines the type that handles the <see cref="Cells"/> instance for the conversion
/// to the RxCy notation.
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
public abstract class RxCyNotation : ICellNotation
{
	/// <summary>
	/// Indicates the regular expression for matching a cell or cell-list.
	/// </summary>
	private static readonly Regex CellOrCellListRegex = new(
		RegularExpressions.CellOrCellList,
		RegexOptions.ExplicitCapture,
		TimeSpan.FromSeconds(5)
	);


	/// <summary>
	/// The <see langword="private"/> instance constructor of this type.
	/// The type is <see langword="static"/>-<see langword="class"/>-like type
	/// so you cannot initialize any instances of this type.
	/// </summary>
	/// <exception cref="NotSupportedException">Always throws.</exception>
	RxCyNotation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string Name => "RxCy";


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToDisplayString(in Cells cells) => ToDisplayString(cells, RxCyNotationOptions.Default);

	/// <summary>
	/// Gets the <see cref="string"/> representation of the current notation
	/// of the specified <see cref="Cells"/> instance, using the specified casing of the capital letters.
	/// </summary>
	/// <param name="cells">
	/// The list of cells to be converted to the <see cref="string"/> representation of the current notation.
	/// </param>
	/// <param name="options">The extra options that controls the output behavior.</param>
	/// <returns>The <see cref="string"/> representation of the current notation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToDisplayString(in Cells cells, in RxCyNotationOptions options)
	{
		const char separator = '|';
		bool upperCasing = options.UpperCasing;
		return cells switch
		{
			[] => string.Empty,
			[var p] => $"{(upperCasing ? 'R' : 'r')}{p / 9 + 1}{(upperCasing ? 'C' : 'c')}{p % 9 + 1}",
			_ => r(cells, upperCasing) is var a && c(cells, upperCasing) is var b && a.Length <= b.Length ? a : b
		};


		static string i(int v) => (v + 1).ToString();

		static unsafe string r(in Cells cells, bool upperCasing)
		{
			var sbRow = new StringHandler(50);
			var dic = new Dictionary<int, List<int>>(9);
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
				sbRow.Append(upperCasing ? 'R' : 'r');
				sbRow.Append(row + 1);
				sbRow.Append(upperCasing ? 'C' : 'c');
				sbRow.AppendRange(dic[row], &i);
				sbRow.Append(separator);
			}
			sbRow.RemoveFromEnd(1);

			return sbRow.ToStringAndClear();
		}

		static unsafe string c(in Cells cells, bool upperCasing)
		{
			var dic = new Dictionary<int, List<int>>(9);
			var sbColumn = new StringHandler(50);
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
				sbColumn.Append(upperCasing ? 'R' : 'r');
				sbColumn.AppendRange(dic[column], &i);
				sbColumn.Append(upperCasing ? 'C' : 'c');
				sbColumn.Append(column + 1);
				sbColumn.Append(separator);
			}
			sbColumn.RemoveFromEnd(1);

			return sbColumn.ToStringAndClear();
		}
	}

	/// <summary>
	/// Parse the specified <see cref="string"/> text, and convert it into the <see cref="Cells"/> instance
	/// as the result value.
	/// </summary>
	/// <param name="str">The <see cref="string"/> text to be parsed.</param>
	/// <returns>
	/// The <see cref="Cells"/> result. If the argument <paramref name="str"/> is <see langword="null"/>
	/// or only contains white spaces, the value will be <see cref="Cells.Empty"/>.
	/// </returns>
	/// <exception cref="FormatException">
	/// Throws when the argument <paramref name="str"/> is malformed.
	/// </exception>
	/// <seealso cref="Cells.Empty"/>
	public static unsafe Cells Parse(string str)
	{
		// Check whether the match is successful.
		if (CellOrCellListRegex.Matches(str) is not [_, ..] matches)
		{
			throw new FormatException("The specified string can't match any cell instance.");
		}

		// Declare the buffer.
		int* bufferRows = stackalloc int[9], bufferColumns = stackalloc int[9];

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
					result.AddAnyway(bufferRows[i] * 9 + bufferColumns[j]);
				}
			}
		}

		// Returns the result.
		return result;
	}

	/// <summary>
	/// Try to parse the specified <see cref="string"/> text, and convert it into the <see cref="Cells"/> instance
	/// as the result value. If failed to parse, the return value will be <see langword="false"/>, but without
	/// any exception thrown.
	/// </summary>
	/// <param name="str">The <see cref="string"/> text to be parsed.</param>
	/// <param name="result">
	/// The result <see cref="Cells"/> instance which are useful if the return value is <see langword="true"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the parse operation succeeded.</returns>
	public static bool TryParse(string str, out Cells result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (FormatException)
		{
			Unsafe.SkipInit(out result);
			return false;
		}
	}
}
