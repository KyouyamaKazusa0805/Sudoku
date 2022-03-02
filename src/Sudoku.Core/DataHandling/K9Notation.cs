using Sudoku.Collections;

namespace Sudoku.DataHandling;

/// <summary>
/// Defines the type that handles the <see cref="Cells"/> instance for the conversion
/// to the K9 notation.
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
public sealed class K9Notation : ICellNotation
{
	/// <summary>
	/// Indicates all possible letters that used in the row notation.
	/// </summary>
	private static readonly char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K' };


	/// <summary>
	/// The <see langword="private"/> instance constructor of this type.
	/// The type is <see langword="static"/>-<see langword="class"/>-like type
	/// so you cannot initialize any instances of this type.
	/// </summary>
	/// <exception cref="NotSupportedException">Always throws.</exception>
	K9Notation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string Name => "K9";


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToDisplayString(in Cells cells) => ToDisplayString(cells, false, false);

	/// <summary>
	/// Gets the <see cref="string"/> representation of the current notation
	/// of the specified <see cref="Cells"/> instance, using the specified casing of the capital letters.
	/// </summary>
	/// <param name="cells">
	/// The list of cells to be converted to the <see cref="string"/> representation of the current notation.
	/// </param>
	/// <param name="upperCasing">
	/// Indicates whether the method should use upper-casing to handle the result notation of cells.
	/// For example, if <see langword="true"/>, the concept "row 3 column 3" will be displayed
	/// as <c>C3</c>; otherwise, <c>c3</c>.
	/// </param>
	/// <param name="avoidConfusionOnRowLetters">
	/// Indicates whether the method should avoid confusion for the letter I and digit 1. For example,
	/// if <see langword="true"/>, row 9 column 9 will be notated as <c>K9</c>; otherwise, <c>I9</c>.
	/// </param>
	/// <returns>The <see cref="string"/> representation of the current notation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToDisplayString(in Cells cells, bool upperCasing, bool avoidConfusionOnRowLetters)
	{
		const char separator = '|';
		return cells switch
		{
			[] => string.Empty,
			[var p] => avoidConfusionOnRowLetters switch
			{
				true => $"{(upperCasing ? Letters[p / 9] : char.ToLower(Letters[p / 9]))}{p % 9 + 1}",
				_ => $"{(upperCasing ? (char)('A' + p / 9) : char.ToLower((char)('A' + p / 9)))}{p % 9 + 1}"
			},
			_ => r(cells, upperCasing, avoidConfusionOnRowLetters) is var a && c(cells, upperCasing, avoidConfusionOnRowLetters) is var b && a.Length <= b.Length ? a : b
		};


		static string i(int v) => (v + 1).ToString();

		static unsafe string r(in Cells cells, bool upperCasing, bool avoidConfusionOnRowLetters)
		{
			var sbRow = new StringHandler(18);
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

		static unsafe string c(in Cells cells, bool upperCasing, bool avoidConfusionOnRowLetters)
		{
			var dic = new Dictionary<int, List<int>>(9);
			var sbColumn = new StringHandler(18);
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
				foreach (int row in dic[column])
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
}
