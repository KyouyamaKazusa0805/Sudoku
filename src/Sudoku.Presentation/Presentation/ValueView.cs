namespace Sudoku.Presentation;

/// <summary>
/// Represents with a value-typed <see cref="View"/> equivalent implementation that displays for a sudoku drawing elements.
/// </summary>
/// <seealso cref="View"/>
public readonly partial struct ValueView : IEnumerable<ViewNode>
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static readonly ValueView Empty = new();


	/// <summary>
	/// Defines a view node segments.
	/// </summary>
	private readonly LinkedList<ViewNodeSegment> _viewNodeSegements = new();


	/// <summary>
	/// Initializes a <see cref="ValueView"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueView()
	{
	}


	/// <summary>
	/// Indicates the number of elements stored in the current collection.
	/// </summary>
	public int Count => _viewNodeSegements.Sum(static s => s.CollectionValuesCount);

	/// <summary>
	/// Indicates the cell nodes that the current data type stored.
	/// </summary>
	public OfTypeEnumerator<CellViewNode> CellNodes => EnumerateNodes<CellViewNode>();

	/// <summary>
	/// Indicates the candidate nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<CandidateViewNode> CandidateNodes => EnumerateNodes<CandidateViewNode>();

	/// <summary>
	/// Indicates the house nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<HouseViewNode> HouseNodes => EnumerateNodes<HouseViewNode>();

	/// <summary>
	/// Indicates the link nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<LinkViewNode> LinkNodes => EnumerateNodes<LinkViewNode>();

	/// <summary>
	/// Indicates the unknown nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<UnknownViewNode> UnknownNodes => EnumerateNodes<UnknownViewNode>();


	/// <summary>
	/// Adds the specified segment into the collection.
	/// </summary>
	/// <param name="segment">The segment.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(ViewNodeSegment segment) => _viewNodeSegements.AddLast(segment);

	/// <summary>
	/// Removes the specified view node instance from the current collection.
	/// </summary>
	/// <param name="node">The node to be removed.</param>
	public void Remove(ViewNode node)
	{
		var (first, last) = (_viewNodeSegements.First, _viewNodeSegements.Last);

		var foundNode = (ViewNode?)null;
		var otherValues = (List<ViewNode>?)null;
		var foundLinkedNode = (LinkedListNode<ViewNodeSegment>?)null;

		for (var tempNode = first; tempNode is not null && !ReferenceEquals(tempNode, last); tempNode = tempNode.Next)
		{
			foundLinkedNode = tempNode;

			switch (tempNode.Value.ActualValue)
			{
				case ViewNode n when node == n:
				{
					foundNode = n;
					goto CheckFoundNode;
				}
				case ViewNode[] array:
				{
					for (var i = 0; i < array.Length; i++)
					{
						var n = array[i];
						if (node == n)
						{
							foundNode = n;
							otherValues = new(array.CopyExcept(i));

							goto CheckFoundNode;
						}
					}
					break;
				}
				case List<ViewNode> list:
				{
					for (var i = 0; i < list.Count; i++)
					{
						var n = list[i];
						if (node == n)
						{
							foundNode = n;
							otherValues = list.CopyExcept(i);

							goto CheckFoundNode;
						}
					}
					break;
				}
			}
		}
	CheckFoundNode:
		if (foundNode is null)
		{
			return;
		}

		var prevLinkedNode = foundLinkedNode!.Previous!;
		_viewNodeSegements.Remove(foundLinkedNode);
		_viewNodeSegements.AddAfter(prevLinkedNode, otherValues!);
	}

	/// <summary>
	/// Enumerates all <see cref="ViewNode"/>s stored in this collection.
	/// </summary>
	/// <returns>An <see cref="Enumerator"/> instance of element type <see cref="ViewNode"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator EnumerateNodes() => new(this);

	/// <summary>
	/// Enumerates all <typeparamref name="TViewNode"/> nodes stored in this collection.
	/// </summary>
	/// <typeparam name="TViewNode">The type of the view node to be fetched and iterated.</typeparam>
	/// <returns>An <see cref="OfTypeEnumerator{TViewNode}"/> instance of element type <see cref="ViewNode"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OfTypeEnumerator<TViewNode> EnumerateNodes<TViewNode>() where TViewNode : ViewNode => new(GetEnumerator());

	/// <summary>
	/// Creates an <see cref="IEnumerable{T}"/> instance of element type <see cref="ViewNodeSegment"/>.
	/// </summary>
	/// <returns>An <see cref="IEnumerable{T}"/> instance of element type <see cref="ViewNodeSegment"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SegmentEnumerator EnumerateSegments() => new(_viewNodeSegements);

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if false
	[IteratorStateMachine(typeof(ValueView))]
#endif
	public Enumerator GetEnumerator() => new(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ViewNode>)this).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<ViewNode> IEnumerable<ViewNode>.GetEnumerator()
	{
		foreach (var segment in _viewNodeSegements)
		{
			switch (segment.ActualValue)
			{
				case ViewNode node:
				{
					yield return node;
					break;
				}
				case ViewNode[] nodes:
				{
					foreach (var node in nodes)
					{
						yield return node;
					}
					break;
				}
				case List<ViewNode> nodes:
				{
					foreach (var node in nodes)
					{
						yield return node;
					}
					break;
				}
			}
		}
	}


	/// <summary>
	/// Defines a basic enumerator that iterates on each <see cref="ViewNode"/>s.
	/// </summary>
	/// <remarks>
	/// This type is decompiled by <see langword="yield"/> statements. For more information please learn C# 2 syntax feature "Yield Statements".
	/// </remarks>
	/// <seealso cref="ViewNode"/>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified view.
		/// </summary>
		/// <param name="view">View.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(ValueView view) => _instance = view;
	}

	/// <summary>
	/// Defines an enumeratot that iterates on each <see cref="ViewNodeSegment"/> instance.
	/// </summary>
	/// <seealso cref="ViewNodeSegment"/>
	public ref partial struct SegmentEnumerator
	{
		/// <summary>
		/// Initializes an <see cref="SegmentEnumerator"/> instance via the specified segments.
		/// </summary>
		/// <param name="segments">Segments.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SegmentEnumerator(LinkedList<ViewNodeSegment> segments) => _enumerator = segments.GetEnumerator();
	}

	/// <summary>
	/// Defines an enumerator that only iterates for a fixed type of <see cref="ViewNode"/>s,
	/// specified as type argument <typeparamref name="TViewNode"/>.
	/// </summary>
	/// <typeparam name="TViewNode">The type of the node to be iterated.</typeparam>
	public ref partial struct OfTypeEnumerator<TViewNode> where TViewNode : ViewNode
	{
		/// <summary>
		/// Initializes an <see cref="OfTypeEnumerator{TViewNode}"/> instance via the specified segments.
		/// </summary>
		/// <param name="enumerator">Enumerator.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal OfTypeEnumerator(Enumerator enumerator) => _enumerator = enumerator;
	}
}
