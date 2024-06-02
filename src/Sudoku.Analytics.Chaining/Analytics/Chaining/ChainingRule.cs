namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a rule that make inferences (strong or weak) between two <see cref="Node"/> instances.
/// </summary>
/// <seealso cref="Node"/>
public abstract class ChainingRule
{
	/// <summary>
	/// Collects for strong links appeared in argument <paramref name="grid"/>
	/// and insert all found values into argument <paramref name="strongLinksDictionary"/>.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="strongLinksDictionary">The collection of strong links, grouped by its node.</param>
	/// <remarks>
	/// Consider adding values from both entries if a link is found.
	/// <see cref="AddBothEntries(Dictionary{Node, SortedSet{Node}}, ref readonly Node, ref readonly Node)"/> is helpful.
	/// </remarks>
	/// <seealso cref="AddBothEntries(Dictionary{Node, SortedSet{Node}}, ref readonly Node, ref readonly Node)"/>
	protected abstract void CollectStrongLinks(ref readonly Grid grid, Dictionary<Node, SortedSet<Node>> strongLinksDictionary);

	/// <summary>
	/// Collects for weak links appeared in argument <paramref name="grid"/>
	/// and insert all found values into argument <paramref name="weakLinksDictionary"/>.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="weakLinksDictionary">The collection of weak links, grouped by its node.</param>
	/// <remarks>
	/// <inheritdoc cref="CollectStrongLinks(ref readonly Grid, Dictionary{Node, SortedSet{Node}})" path="/remarks"/>
	/// </remarks>
	/// <seealso cref="AddBothEntries(Dictionary{Node, SortedSet{Node}}, ref readonly Node, ref readonly Node)"/>
	protected abstract void CollectWeakLinks(ref readonly Grid grid, Dictionary<Node, SortedSet<Node>> weakLinksDictionary);

	/// <summary>
	/// Add <see cref="Node"/> to both entries.
	/// </summary>
	/// <param name="linksDictionary">The dictionary used by adding operation.</param>
	/// <param name="node1">The first node to be added.</param>
	/// <param name="node2">The second node to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void AddBothEntries(Dictionary<Node, SortedSet<Node>> linksDictionary, ref readonly Node node1, ref readonly Node node2)
	{
		var c = new SortedSet<Node>();
		c.AddRef(in node2);
		if (!linksDictionary.TryAdd(node1, c))
		{
			linksDictionary[node1].AddRef(in node2);
		}

		var d = new SortedSet<Node>();
		d.AddRef(in node1);
		if (!linksDictionary.TryAdd(node2, d))
		{
			linksDictionary[node2].AddRef(in node1);
		}
	}
}
