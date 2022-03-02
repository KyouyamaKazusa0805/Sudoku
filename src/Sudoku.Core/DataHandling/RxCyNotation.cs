using Sudoku.Collections;

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
	public static string ToDisplayString(in Cells cells) => ToDisplayString(cells, false);

	/// <summary>
	/// Gets the <see cref="string"/> representation of the current notation
	/// of the specified <see cref="Cells"/> instance, using the specified casing of the capital letters.
	/// </summary>
	/// <param name="cells">
	/// The list of cells to be converted to the <see cref="string"/> representation of the current notation.
	/// </param>
	/// <param name="upperCasing">
	/// Indicates whether we should use upper-casing to handle the result notation of cells.
	/// For example, if <see langword="true"/>, the concept "row 3 column 3" will be displayed
	/// as <c>R3C3</c>; otherwise, <c>r3c3</c>.
	/// </param>
	/// <returns>The <see cref="string"/> representation of the current notation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToDisplayString(in Cells cells, bool upperCasing)
	{
		const char separator = '|';
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
}
