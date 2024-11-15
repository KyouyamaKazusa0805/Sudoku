namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridNodeExtensions
{
	/// <summary>
	/// Treat the specified node as a real conclusion, and apply to a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="node">The node.</param>
	public static void Apply(this ref Grid grid, Node node)
	{
		ref readonly var map = ref node.Map;
		if (node.IsOn)
		{
			// Find intersections to be removed; or assign it directly if the node only uses 1 candidate.
			if (map is [var onlyCandidate])
			{
				grid.SetDigit(onlyCandidate / 9, onlyCandidate % 9);
			}
			else
			{
				foreach (var candidate in map.PeerIntersection)
				{
					if (grid.GetState(candidate / 9) == CellState.Empty)
					{
						grid[candidate / 9] &= (Mask)~(1 << candidate % 9);
					}
				}
			}
		}
		else
		{
			// If a node is considered as false, we should remove all possible candidates of the node.
			foreach (var candidate in map)
			{
				grid[candidate / 9] &= (Mask)~(1 << candidate % 9);
			}
		}
	}
}
