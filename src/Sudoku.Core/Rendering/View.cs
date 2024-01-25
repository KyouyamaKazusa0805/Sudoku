namespace Sudoku.Rendering;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
public sealed partial class View : HashSet<ViewNode>
{
	/// <summary>
	/// Adds a list of <see cref="ViewNode"/>s into the collection.
	/// </summary>
	/// <param name="nodes">A list of <see cref="ViewNode"/> instance.</param>
	public void AddRange<T>(scoped ReadOnlySpan<T> nodes) where T : ViewNode
	{
		foreach (var node in nodes)
		{
			Add(node);
		}
	}

	/// <summary>
	/// Determines whether the specified <see cref="View"/> stores several <see cref="BabaGroupViewNode"/>s,
	/// and at least one of it overlaps the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating whether being overlapped.</returns>
	public bool UnknownOverlaps(Cell cell)
	{
		foreach (var babaGroupNode in this.OfType<BabaGroupViewNode>())
		{
			if (babaGroupNode.Cell == cell)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Creates a new <see cref="View"/> instance with same values as the current instance, with independency.
	/// </summary>
	/// <returns>A new <see cref="View"/> instance with same values as the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View Clone() => Count == 0 ? [] : [.. from node in this select node.Clone()];
}
