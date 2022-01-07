namespace Sudoku.Data.Models;

/// <summary>
/// Provides extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExtensions
{
	/// <summary>
	/// Converts the current grid into the <see cref="CellInfo"/> list.
	/// </summary>
	/// <param name="this">The current grid.</param>
	/// <returns>The list of <see cref="CellInfo"/> instances.</returns>
	public static CellInfo[] ToCellInfoList(this in Grid @this)
	{
		var result = new CellInfo[81];
		for (int i = 0; i < 81; i++)
		{
			result[i] = CellInfo.GetCellInfo(@this, i);
		}

		return result;
	}
}
