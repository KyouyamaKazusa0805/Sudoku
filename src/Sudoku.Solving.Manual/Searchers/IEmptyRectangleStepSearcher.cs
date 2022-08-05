namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Empty Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Empty Rectangle</item>
/// </list>
/// </summary>
public interface IEmptyRectangleStepSearcher : ISingleDigitPatternStepSearcher
{
	/// <summary>
	/// Determine whether the specified cells in the specified block form an empty rectangle.
	/// </summary>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="block">The block where the cells may form an empty rectangle structure.</param>
	/// <param name="row">The row that the empty rectangle used.</param>
	/// <param name="column">The column that the empty rectangle used.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating that. If <see langword="true"/>,
	/// both arguments <paramref name="row"/> and <paramref name="column"/> can be used;
	/// otherwise, both arguments should be discards.
	/// </returns>
	protected internal static sealed bool IsEmptyRectangle(scoped in Cells cells, int block, out int row, out int column)
	{
		int r = block / 3 * 3 + 9, c = block % 3 * 3 + 18;
		for (int i = r, count = 0, rPlus3 = r + 3; i < rPlus3; i++)
		{
			if ((cells & HouseMaps[i]) is not [] || ++count <= 1)
			{
				continue;
			}

			row = column = -1;
			return false;
		}

		for (int i = c, count = 0, cPlus3 = c + 3; i < cPlus3; i++)
		{
			if ((cells & HouseMaps[i]) is not [] || ++count <= 1)
			{
				continue;
			}

			row = column = -1;
			return false;
		}

		for (int i = r, rPlus3 = r + 3; i < rPlus3; i++)
		{
			for (int j = c, cPlus3 = c + 3; j < cPlus3; j++)
			{
				if (cells >> (HouseMaps[i] | HouseMaps[j]))
				{
					continue;
				}

				row = i;
				column = j;
				return true;
			}
		}

		row = column = -1;
		return false;
	}
}
