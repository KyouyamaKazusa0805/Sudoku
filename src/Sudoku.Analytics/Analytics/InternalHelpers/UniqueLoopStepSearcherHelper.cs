namespace Sudoku.Analytics.InternalHelpers;

/// <summary>
/// Used by <see cref="UniqueLoopStepSearcher"/>.
/// </summary>
/// <seealso cref="UniqueLoopStepSearcher"/>
internal static class UniqueLoopStepSearcherHelper
{
	/// <summary>
	/// Determine whether the specified loop is a valid unique loop or unique rectangle pattern.
	/// </summary>
	/// <param name="loopPath">The path of the loop.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool IsValidLoop(scoped in ValueList<Cell> loopPath)
	{
		var visitedOdd = 0;
		var visitedEven = 0;

		var isOdd = false;
		foreach (var cell in loopPath)
		{
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouseIndex(houseType);
				if (isOdd)
				{
					if ((visitedOdd >> house & 1) != 0)
					{
						return false;
					}

					visitedOdd |= 1 << house;
				}
				else
				{
					if ((visitedEven >> house & 1) != 0)
					{
						return false;
					}

					visitedEven |= 1 << house;
				}
			}

			isOdd = !isOdd;
		}

		return visitedEven == visitedOdd;
	}

	/// <summary>
	/// Determine whether a loop is a generalized unique loop.
	/// </summary>
	/// <param name="loop">The loop to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool IsGeneralizedUniqueLoop(scoped in CellMap loop)
	{
		_ = loop is { Count: var length, Houses: var houses, RowMask: var r, ColumnMask: var c, BlockMask: var b };
		if ((length & 1) != 0 || length < 6)
		{
			return false;
		}

		var halfLength = length >> 1;
		if (PopCount((uint)r) != halfLength || PopCount((uint)c) != halfLength || PopCount((uint)b) != halfLength)
		{
			return false;
		}

		foreach (var house in houses)
		{
			if ((HousesMap[house] & loop).Count != 2)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Gets all possible links of the loop.
	/// </summary>
	/// <param name="path">The loop, specified as its path.</param>
	/// <returns>A list of <see cref="LinkViewNode"/> instances.</returns>
	public static List<LinkViewNode> GetLoopLinks(Cell[] path)
	{
		const Digit digit = 4;
		var result = new List<LinkViewNode>();
		for (var i = 0; i < path.Length; i++)
		{
			result.Add(
				new(
					WellKnownColorIdentifierKind.Normal,
					new(digit, CellsMap[path[i]]),
					new(digit, CellsMap[path[i + 1 == path.Length ? 0 : i + 1]]),
					Inference.ConjugatePair
				)
			);
		}

		return result;
	}
}
