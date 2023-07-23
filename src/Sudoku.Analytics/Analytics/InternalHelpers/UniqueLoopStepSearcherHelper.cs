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
	/// <remarks>
	/// <para>
	/// This method uses a trick way (node-coloring) to check whether a loop is a unique loop.
	/// The parameter <paramref name="loopPath"/> holds a list of cells, which are nodes of the loop.
	/// Then we begin to colorize each node with 2 different colors <b><i>A</i></b> and <b><i>B</i></b>.
	/// The only point we should notice is that the color between 2 adjacent nodes should be different
	/// (i.e. one is colored with <b><i>A</i></b> and the other should be colored with <b><i>B</i></b>).
	/// If at least one pair of cells in a same house are colored with a same color, it won't be a valid unique loop.
	/// Those colors stands for the final filling digits. Therefore, "Two cells in a same house are colored same"
	/// means "Two cells in a same house are filled with a same digit", which disobeys the basic sudoku rule.
	/// </para>
	/// <para>
	/// This method won't check for whether the loop is a unique rectangle (of length 4). It means, a valid unique rectangle
	/// can also make this method return <see langword="true"/>.
	/// </para>
	/// </remarks>
	public static bool IsValidLoop(scoped in ValueList<Cell> loopPath)
	{
		var (visitedOdd, visitedEven, isOdd) = (0, 0, false);
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
	/// <remarks>
	/// <para>This method uses another way to check for unique loops.</para>
	/// <para>
	/// <i>
	/// However, this method contains a little bug for checking loops, leading to returning <see langword="true"/> for this method,
	/// and returning <see langword="false"/> for the method <see cref="IsValidLoop(in ValueList{int})"/> above this. If a pattern is like:
	/// </i>
	/// <code><![CDATA[
	/// .-----.-----.
	/// | x   | x   |
	/// |   x |   x |
	/// |-----+-----|
	/// | x   |   x |
	/// |   x | x   |
	/// '-----'-----'
	/// ]]></code>
	/// <i>This pattern isn't a valid unique loop, because the pattern has no suitable way to filling digits, without conflict.</i>
	/// </para>
	/// <para>
	/// This method can also check for separated ones, e.g.:
	/// <code><![CDATA[
	/// .-------.-------.-------.
	/// | x     | x     |       |
	/// | x     | x     |       |
	/// |       |       |       |
	/// |-------+-------+-------|
	/// |       |       |       |
	/// |       |     x |     x |
	/// |       |     x |     x |
	/// ~~~~~~~~~~~~~~~~~~~~~~~~~
	/// ]]></code>
	/// </para>
	/// <para><inheritdoc cref="IsValidLoop(in ValueList{int})" path="//remarks/para[2]"/></para>
	/// </remarks>
	/// <seealso cref="IsValidLoop(in ValueList{int})"/>
	public static bool IsGeneralizedUniqueLoop(scoped in CellMap loop)
	{
		// The length of the loop pattern must be at least 4, and an even.
		_ = loop is { Count: var length, Houses: var houses, RowMask: var r, ColumnMask: var c, BlockMask: var b };
		if ((length & 1) != 0 || length < 4)
		{
			return false;
		}

		// The pattern must span n/2 rows, n/2 columns and n/2 blocks, and n is the length of the pattern).
		var halfLength = length >> 1;
		if (PopCount((uint)r) != halfLength || PopCount((uint)c) != halfLength || PopCount((uint)b) != halfLength)
		{
			return false;
		}

		// All houses spanned should contain only 2 cells of the pattern.
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
					WellKnownColorIdentifier.Normal,
					new(digit, CellsMap[path[i]]),
					new(digit, CellsMap[path[i + 1 == path.Length ? 0 : i + 1]]),
					Inference.ConjugatePair
				)
			);
		}

		return result;
	}
}
