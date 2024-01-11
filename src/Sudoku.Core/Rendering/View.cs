namespace Sudoku.Rendering;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
public sealed partial class View : HashSet<ViewNode>
{
	/// <inheritdoc cref="AddRange(IEnumerable{ViewNode})"/>
	/// <typeparam name="T">The type of the node.</typeparam>
	public void AddRange<T>(scoped ReadOnlySpan<T> nodes) where T : ViewNode
	{
		foreach (var node in nodes)
		{
			Add(node);
		}
	}

	/// <summary>
	/// Adds a list of <see cref="ViewNode"/>s into the collection.
	/// </summary>
	/// <param name="nodes">A list of <see cref="ViewNode"/> instance.</param>
	public void AddRange(IEnumerable<ViewNode> nodes)
	{
		foreach (var node in nodes)
		{
			Add(node);
		}
	}

	/// <summary>
	/// Searches for an element that matches the conditions defined by the specified predicate,
	/// and returns the first occurrence within the entire <see cref="View"/>.
	/// </summary>
	/// <param name="match">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
	/// <returns>
	/// The first element that matches the conditions defined by the specified predicate, if found; otherwise, <see langword="null"/>.
	/// </returns>
	public ViewNode? Find(Func<ViewNode, bool> match)
	{
		foreach (var element in this)
		{
			if (match(element))
			{
				return element;
			}
		}

		return null;
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
