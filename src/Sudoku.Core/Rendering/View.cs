namespace Sudoku.Rendering;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
public sealed partial class View : HashSet<ViewNode>, ICloneable<View>
{
	/// <summary>
	/// Creates an empty <see cref="View"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private View() : base()
	{
	}

	/// <summary>
	/// Initializes a <see cref="View"/> instance via the specified list as the raw value.
	/// </summary>
	/// <param name="nodes">The list as the raw value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private View(HashSet<ViewNode> nodes) : base(nodes)
	{
	}


	/// <summary>
	/// Indicates the basic nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<BasicViewNode> BasicNodes => OfType<BasicViewNode>();

	/// <summary>
	/// Indicates icon nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<IconViewNode> FigureNodes => OfType<IconViewNode>();

	/// <summary>
	/// Indicates the shape view nodes.
	/// </summary>
	public OfTypeEnumerator<ShapeViewNode> ShapeViewNodes => OfType<ShapeViewNode>();

	/// <summary>
	/// Indicates the grouped view nodes.
	/// </summary>
	public OfTypeEnumerator<GroupedViewNode> GroupedViewNodes => OfType<GroupedViewNode>();


	/// <summary>
	/// Indicates the empty instance.
	/// </summary>
	public static View Empty => new();


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
	/// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
	/// <returns>
	/// The first element that matches the conditions defined by the specified predicate, if found; otherwise, <see langword="null"/>.
	/// </returns>
	public ViewNode? Find(Predicate<ViewNode> match)
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
	/// Filters the view nodes, only returns nodes of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	/// <returns>The target collection of element type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OfTypeEnumerator<T> OfType<T>() where T : ViewNode => new(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View Clone()
	{
		return Count == 0 ? Empty : new(cloneNodes());


		HashSet<ViewNode> cloneNodes()
		{
			var result = new HashSet<ViewNode>(Count);
			foreach (var node in this)
			{
				result.Add(node.Clone());
			}

			return result;
		}
	}


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
