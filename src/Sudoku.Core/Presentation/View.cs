namespace Sudoku.Presentation;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
public sealed class View : ICloneable, IEnumerable<ViewNode>
{
	/// <summary>
	/// Indicates the inner dictionary.
	/// </summary>
	private readonly List<ViewNode> _nodes = new();


	/// <summary>
	/// Creates an empty <see cref="View"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View()
	{
	}

	/// <summary>
	/// Initializes a <see cref="View"/> instance via the specified list as the raw value.
	/// </summary>
	/// <param name="nodes">The list as the raw value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private View(List<ViewNode> nodes) => _nodes = nodes;


	/// <summary>
	/// Indicates the number of elements stored in the current collection.
	/// </summary>
	public int Count => _nodes.Count;


	/// <summary>
	/// Gets the <see cref="ViewNode"/> at the specified position.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The <see cref="ViewNode"/> instance.</returns>
	public ViewNode this[int index] => _nodes[index];


	/// <summary>
	/// Adds the specified <see cref="ViewNode"/> into the collection.
	/// </summary>
	/// <param name="node">The <see cref="ViewNode"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(ViewNode node)
	{
		if (!_nodes.Contains(node))
		{
			_nodes.Add(node);
		}
	}

	/// <summary>
	/// Adds a serial of elements into the collection.
	/// </summary>
	/// <param name="nodes">The <see cref="ViewNode"/> instances to be added.</param>
	public void AddRange(IEnumerable<ViewNode> nodes)
	{
		foreach (var node in nodes)
		{
			Add(node);
		}
	}

	/// <summary>
	/// Determines whether an element is in the current collection.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(ViewNode node) => _nodes.Contains(node);

	/// <inheritdoc cref="ICloneable.Clone"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View Clone() => Count == 0 ? new() : new(new(from node in _nodes select node.Clone()));

	/// <summary>
	/// Adds a serial of cells to be highlighted.
	/// </summary>
	/// <param name="highlightedCells">The highlighted cells.</param>
	/// <returns>The reference that is same as <see langword="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View AddCells(params (Identifier Identifier, int Cell)[] highlightedCells)
	{
		_nodes.AddRange(from tuple in highlightedCells select new CellViewNode(tuple.Identifier, tuple.Cell));
		return this;
	}

	/// <summary>
	/// Adds a serial of candidates to be highlighted.
	/// </summary>
	/// <param name="highlightedCandidates">The highlighted candidates.</param>
	/// <returns>The reference that is same as <see langword="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View AddCandidates(params (Identifier Identifier, int Candidate)[] highlightedCandidates)
	{
		_nodes.AddRange(
			from tuple in highlightedCandidates
			select new CandidateViewNode(tuple.Identifier, tuple.Candidate));
		return this;
	}

	/// <summary>
	/// Adds a serial of regions to be highlighted.
	/// </summary>
	/// <param name="highlightedRegions">The highlighted regions.</param>
	/// <returns>The reference that is same as <see langword="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View AddRegions(params (Identifier Identifier, int Region)[] highlightedRegions)
	{
		_nodes.AddRange(
			from tuple in highlightedRegions
			select new RegionViewNode(tuple.Identifier, tuple.Region));
		return this;
	}

	/// <summary>
	/// Adds a serial of links to be highlighted.
	/// </summary>
	/// <param name="highlightedLinks">The highlighted links.</param>
	/// <returns>The reference that is same as <see langword="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View AddLink(params (Identifier Identifier, LockedTarget Start, LockedTarget End, LinkKind LinkKind)[] highlightedLinks)
	{
		_nodes.AddRange(
			from tuple in highlightedLinks
			select new LinkViewNode(tuple.Identifier, tuple.Start, tuple.End, tuple.LinkKind));
		return this;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public List<ViewNode>.Enumerator GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	object ICloneable.Clone() => Clone();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<ViewNode> IEnumerable<ViewNode>.GetEnumerator() => GetEnumerator();
}
