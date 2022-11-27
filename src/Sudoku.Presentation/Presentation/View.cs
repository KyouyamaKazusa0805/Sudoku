namespace Sudoku.Presentation;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
public sealed class View : ICloneable<View>, IEnumerable<ViewNode>
{
	/// <summary>
	/// Indicates the inner dictionary.
	/// </summary>
	private readonly List<ViewNode> _nodes = new();


	/// <summary>
	/// Creates an empty <see cref="View"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private View()
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
	/// Indicates the cell nodes that the current data type stored.
	/// </summary>
	public IEnumerable<CellViewNode> CellNodes => _nodes.OfType<CellViewNode>();

	/// <summary>
	/// Indicates the candidate nodes that the current data type stores.
	/// </summary>
	public IEnumerable<CandidateViewNode> CandidateNodes => _nodes.OfType<CandidateViewNode>();

	/// <summary>
	/// Indicates the house nodes that the current data type stores.
	/// </summary>
	public IEnumerable<HouseViewNode> HouseNodes => _nodes.OfType<HouseViewNode>();

	/// <summary>
	/// Indicates the link nodes that the current data type stores.
	/// </summary>
	public IEnumerable<LinkViewNode> LinkNodes => _nodes.OfType<LinkViewNode>();

	/// <summary>
	/// Indicates the unknown nodes that the current data type stores.
	/// </summary>
	public IEnumerable<UnknownViewNode> UnknownNodes => _nodes.OfType<UnknownViewNode>();

	/// <summary>
	/// Indicates the border bar nodes that the current data type stores.
	/// </summary>
	public IEnumerable<BorderBarViewNode> BorderBarNodes => _nodes.OfType<BorderBarViewNode>();

	/// <summary>
	/// Indicates the Kropki dot nodes that the current data type stores.
	/// </summary>
	public IEnumerable<KropkiDotViewNode> KropkiDotNodes => _nodes.OfType<KropkiDotViewNode>();

	/// <summary>
	/// Indicates the greater-than sign nodes that the current data type stores.
	/// </summary>
	public IEnumerable<GreaterThanSignViewNode> GreaterThanNodes => _nodes.OfType<GreaterThanSignViewNode>();

	/// <summary>
	/// Indicates the XV sign nodes that the current data type stores.
	/// </summary>
	public IEnumerable<XvSignViewNode> XvNodes => _nodes.OfType<XvSignViewNode>();


	/// <summary>
	/// Indicates the empty instance.
	/// </summary>
	public static View Empty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new();
	}


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
	/// Removes the specified <see cref="ViewNode"/> from the collection.
	/// </summary>
	/// <param name="node">The <see cref="ViewNode"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(ViewNode node)
	{
		if (_nodes.Contains(node))
		{
			_nodes.Remove(node);
		}
	}

	/// <summary>
	/// Determines whether an element is in the current collection.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(ViewNode node) => _nodes.Contains(node);

	/// <summary>
	/// Determines whether an element satisfying the specified condition is in the current collection.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>If found, the first found node will be returned; otherwise, <see langword="null"/>.</returns>
	public ViewNode? Contains(Predicate<ViewNode> predicate)
	{
		for (var i = 0; i < _nodes.Count; i++)
		{
			var node = _nodes[i];
			if (predicate(node))
			{
				return node;
			}
		}

		return null;
	}

	/// <summary>
	/// <para>Determines whether the current view contains a view node using the specified candidate value.</para>
	/// <para>This method will be useful for cannibalism checking cases.</para>
	/// </summary>
	/// <param name="candidate">The candidate to be determined.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ConflictWith(int candidate) => CandidateNodes.Any(n => n.Candidate == candidate);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View Clone() => Count == 0 ? Empty : new(new(from node in _nodes select node.Clone()));

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public List<ViewNode>.Enumerator GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<ViewNode> IEnumerable<ViewNode>.GetEnumerator() => _nodes.GetEnumerator();


	/// <summary>
	/// Adds a new node into the collection.
	/// </summary>
	/// <param name="originalView">The original view.</param>
	/// <param name="newNode">The new item to be added.</param>
	/// <returns>The reference that is same as the argument <paramref name="originalView"/>.</returns>
	/// <remarks>
	/// Please note that the operator is mutable one, which means the appending operation
	/// is based on the argument <paramref name="originalView"/>.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static View operator |(View originalView, ViewNode? newNode)
	{
		if (newNode is not null)
		{
			originalView.Add(newNode);
		}

		return originalView;
	}

	/// <inheritdoc cref="op_BitwiseOr(View, IEnumerable{ViewNode})"/>
	public static View operator |(View originalView, ViewNode[]? highlightedItems)
	{
		if (highlightedItems is null)
		{
			return originalView;
		}

		foreach (var node in highlightedItems)
		{
			originalView.Add(node);
		}

		return originalView;
	}

	/// <summary>
	/// Adds a serial of view nodes into the collection.
	/// </summary>
	/// <param name="originalView">The original view.</param>
	/// <param name="highlightedItems">The highlighted items.</param>
	/// <returns>The reference that is same as the argument <paramref name="originalView"/>.</returns>
	/// <remarks>
	/// Please note that the operator is mutable one, which means the appending operation
	/// is based on the argument <paramref name="originalView"/>.
	/// </remarks>
	public static View operator |(View originalView, IEnumerable<ViewNode>? highlightedItems)
	{
		if (highlightedItems is null)
		{
			return originalView;
		}

		foreach (var node in highlightedItems)
		{
			originalView.Add(node);
		}

		return originalView;
	}
}
