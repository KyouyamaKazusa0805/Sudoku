#define ENHANCED_DRAWING_APIS

namespace Sudoku.Presentation;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
public sealed partial class View : ICloneable<View>, IEnumerable<ViewNode>
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
	/// Indicates the basic nodes that the current data type stores.
	/// </summary>
	public OfTypeIterator<BasicViewNode> BasicNodes => OfType<BasicViewNode>();

	/// <summary>
	/// Indicates figure nodes that the current data type stores.
	/// </summary>
	public OfTypeIterator<FigureViewNode> FigureNodes => OfType<FigureViewNode>();

#if ENHANCED_DRAWING_APIS
	/// <summary>
	/// Indicates the shape view nodes.
	/// </summary>
	public OfTypeIterator<ShapeViewNode> ShapeViewNodes => OfType<ShapeViewNode>();

	/// <summary>
	/// Indicates the grouped view nodes.
	/// </summary>
	public OfTypeIterator<GroupedViewNode> GroupedViewNodes => OfType<GroupedViewNode>();
#endif


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
	public ViewNode this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _nodes[index];
	}


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
	/// <para>Determines whether the current view contains a view node using the specified candidate value.</para>
	/// <para>This method will be useful for cannibalism checking cases.</para>
	/// </summary>
	/// <param name="candidate">The candidate to be determined.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ConflictWith(int candidate) => OfType<CandidateViewNode>().Any(n => n.Candidate == candidate);

	/// <summary>
	/// Determines whether the current collection contains a <see cref="ViewNode"/> instance whose value is considered equal
	/// with the specified node.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Contains(ViewNode node) => Exists(element => element == node, out _);

	/// <summary>
	/// Determines whether the current collection contains a <see cref="ViewNode"/> instance whose value satisfies the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to check for each node.</param>
	/// <param name="node">The found node.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Exists(Predicate<ViewNode> predicate, [NotNullWhen(true)] out ViewNode? node)
	{
		foreach (var element in _nodes)
		{
			if (predicate(element))
			{
				node = element;
				return true;
			}
		}

		node = null;
		return false;
	}

	/// <summary>
	/// Projects the collection, converting it into a new collection whose elements is converted by the specified method.
	/// </summary>
	/// <param name="selector">The selector.</param>
	/// <returns>The target iterator.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SelectIterator<T> Select<T>(Func<ViewNode, T> selector) => new(this, selector);

	/// <summary>
	/// Filters the collection by specified condition.
	/// </summary>
	/// <param name="selector">The selector.</param>
	/// <returns>The target iterator.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public WhereIterator Where(Func<ViewNode, bool> selector) => new(this, selector);

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Iterator GetEnumerator() => new(this);

	/// <summary>
	/// Filters the view nodes, only returns nodes of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	/// <returns>The target collection of element type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OfTypeIterator<T> OfType<T>() where T : ViewNode => new(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View Clone() => Count == 0 ? Empty : new(new(from node in _nodes select node.Clone()));

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


	/// <summary>
	/// Represents with a default enumerator type that provides the mechanism of elementary operations
	/// used by <see langword="foreach"/> statements.
	/// </summary>
	public ref partial struct Iterator
	{
		/// <summary>
		/// Initializes an <see cref="Iterator"/> instance via the specified list of nodes.
		/// </summary>
		/// <param name="view">The internal nodes.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Iterator(View view) => _enumerator = view._nodes.GetEnumerator();
	}

	/// <summary>
	/// Represents an enumerator that iterates for a list of elements that is projected by the current collection,
	/// converting by the specified converter.
	/// </summary>
	/// <typeparam name="T">The type of projected elements.</typeparam>
	public ref partial struct SelectIterator<T>
	{
		/// <summary>
		/// Initializes an <see cref="SelectIterator{T}"/> instance via the specified list of nodes.
		/// </summary>
		/// <param name="view">The internal nodes.</param>
		/// <param name="selector">The selector.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SelectIterator(View view, Func<ViewNode, T> selector) => (_enumerator, _selector) = (view._nodes.GetEnumerator(), selector);
	}

	/// <summary>
	/// Represents an enumerator that iterates for view nodes satisfying the specified condition.
	/// </summary>
	public ref partial struct WhereIterator
	{
		/// <summary>
		/// Initializes an <see cref="WhereIterator"/> instance via the specified list of nodes.
		/// </summary>
		/// <param name="view">The internal nodes.</param>
		/// <param name="filteringCondition">The condition.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal WhereIterator(View view, Func<ViewNode, bool> filteringCondition)
			=> (_enumerator, _filteringCondition) = (view._nodes.GetEnumerator(), filteringCondition);
	}

	/// <summary>
	/// Represents an enumerator that iterates for <typeparamref name="T"/>-typed instances.
	/// </summary>
	/// <typeparam name="T">The type of the element node.</typeparam>
	public ref partial struct OfTypeIterator<T> where T : ViewNode
	{
		/// <summary>
		/// Initializes an <see cref="OfTypeIterator{T}"/> instance via the specified list of nodes.
		/// </summary>
		/// <param name="view">The internal nodes.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal OfTypeIterator(View view)
		{
			_count = view._nodes.Count;
			_enumerator = view._nodes.GetEnumerator();
		}
	}
}
