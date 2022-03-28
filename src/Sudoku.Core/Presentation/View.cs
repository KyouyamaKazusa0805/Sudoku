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
	public View Clone() => Count == 0 ? Empty : new(new(from node in _nodes select node.Clone()));

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
	public static View operator +(View originalView, ViewNode? newNode)
	{
		if (newNode is not null)
		{
			originalView.Add(newNode);
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static View operator +(View originalView, IEnumerable<ViewNode>? highlightedItems)
	{
		if (highlightedItems is not null)
		{
			originalView.AddRange(highlightedItems);
		}

		return originalView;
	}
}
