namespace Sudoku.Concepts;

/// <summary>
/// Represents a list of methods that operates on <see cref="Cell"/>, <see cref="Candidate"/>, <see cref="House"/> instances,
/// with interactions on <see cref="CellMap"/>, <see cref="CandidateMap"/> and <see cref="Grid"/>, to describe subview-related logic.
/// </summary>
public static class Subview
{
	/// <summary>
	/// Reduces the <see cref="CellMap"/> instances, only checks for cells in the specified cells, and merge into a <see cref="Mask"/> value.
	/// </summary>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="house">The house to be checked.</param>
	/// <returns>A <see cref="Mask"/> instance.</returns>
	public static Mask ReduceCellByHouse(ref readonly CellMap cells, House house)
	{
		var (result, i) = ((Mask)0, 0);
		foreach (var cell in HousesCells[house])
		{
			if (cells.Contains(cell))
			{
				result |= (Mask)(1 << i);
			}
			i++;
		}
		return result;
	}

	/// <summary>
	/// Expands the current <see cref="CellMap"/> instance, inserting into a <see cref="CandidateMap"/> instance by specified digit.
	/// </summary>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <returns>A <see cref="CandidateMap"/> instance.</returns>
	public static CandidateMap ExpandedCellFromDigit(ref readonly CellMap cells, Digit digit)
	{
		var result = CandidateMap.Empty;
		foreach (var cell in cells.Offsets)
		{
			result.Add(cell * 9 + digit);
		}
		return result;
	}

	/// <summary>
	/// Reduces the <see cref="CandidateMap"/> instance, only checks for candidates whose digit is equal to argument <paramref name="digit"/>,
	/// and merge into a <see cref="CellMap"/> value.
	/// </summary>
	/// <param name="candidates">The candidates to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	public static CellMap ReduceCandidateByDigit(ref readonly CandidateMap candidates, Digit digit)
	{
		var result = CellMap.Empty;
		foreach (var element in candidates)
		{
			if (element % 9 == digit)
			{
				result.Add(element / 9);
			}
		}
		return result;
	}
}
