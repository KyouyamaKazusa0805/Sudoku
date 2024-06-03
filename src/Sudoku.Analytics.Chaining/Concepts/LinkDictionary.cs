namespace Sudoku.Concepts;

/// <summary>
/// Represents a list of <see cref="Node"/> relations on strong or weak links.
/// </summary>
/// <seealso cref="Node"/>
public sealed class LinkDictionary : Dictionary<Node, HashSet<Node>>
{
	/// <summary>
	/// Add a link to the current collection with both entries on nodes of the link used.
	/// </summary>
	/// <param name="node1">Indicates the first node to be added.</param>
	/// <param name="node2">Indicates the second node to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddEntry(Node node1, Node node2)
	{
		if (!TryAdd(node1, [node2]))
		{
			this[node1].Add(node2);
		}
		if (!TryAdd(node2, [node1]))
		{
			this[node2].Add(node1);
		}
	}
}
