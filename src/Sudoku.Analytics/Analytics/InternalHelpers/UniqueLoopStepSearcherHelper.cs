namespace Sudoku.Analytics.InternalHelpers;

/// <summary>
/// Used by <see cref="UniqueLoopStepSearcher"/>.
/// </summary>
/// <seealso cref="UniqueLoopStepSearcher"/>
internal static class UniqueLoopStepSearcherHelper
{
	/// <summary>
	/// Defines a templating method that can determine whether a loop is a valid unique loop.
	/// </summary>
	/// <param name="loop">The loop to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool IsUniqueLoopOrSeparated(scoped in CellMap loop)
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
}
