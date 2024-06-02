namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a rule that make inferences (strong or weak) between two <see cref="Node"/> instances.
/// </summary>
/// <seealso cref="Node"/>
public abstract class ChainingRule
{
	/// <summary>
	/// Collects for strong links appeared in argument <paramref name="grid"/>
	/// and insert all found values into argument <paramref name="linkDictionary"/>.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="linkDictionary">The collection of strong links, grouped by its node.</param>
	/// <remarks>
	/// Consider adding values from both entries if a link is found.
	/// The method call <see cref="LinkDictionary.AddEntry(Node, Node)"/> is helpful.
	/// </remarks>
	/// <seealso cref="LinkDictionary.AddEntry(Node, Node)"/>
	public abstract void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary);

	/// <summary>
	/// Collects for weak links appeared in argument <paramref name="grid"/>
	/// and insert all found values into argument <paramref name="linkDictionary"/>.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="linkDictionary">The collection of weak links, grouped by its node.</param>
	/// <remarks>
	/// <inheritdoc cref="CollectStrongLinks(ref readonly Grid, LinkDictionary)" path="/remarks"/>
	/// </remarks>
	/// <seealso cref="LinkDictionary.AddEntry(Node, Node)"/>
	public abstract void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary);
}
