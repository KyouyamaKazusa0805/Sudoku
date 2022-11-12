namespace Sudoku.Presentation;

/// <summary>
/// Represents with a value-typed <see cref="View"/> equivalent implementation that displays for a sudoku drawing elements.
/// </summary>
/// <seealso cref="View"/>
public readonly struct ValueView : IEnumerable<ViewNode>
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
	public void Remove(ViewNode node) => throw new NotImplementedException("This method will be implemented later.");

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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[IteratorStateMachine(typeof(ValueView))]
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
	public ref struct Enumerator
	{
		/// <summary>
		/// The <see cref="ValueView"/> instance.
		/// </summary>
		private readonly ValueView _instance;

		/// <summary>
		/// The state of the iteration. The value can be both negative and positive.
		/// </summary>
		private int _state;

		/// <summary>
		/// Records the index having been iterated. <b>This field is only used for the case that the current segment is an array.</b>
		/// </summary>
		private int _arrayIteratedIndex;

		/// <summary>
		/// Indicates the array to be iterated. <b>This field is only used for the case that the current segment is an array.</b>
		/// </summary>
		private ViewNode[]? _array;

		/// <summary>
		/// Indicates the target enumerator used for the whole iteration.
		/// </summary>
		private LinkedList<ViewNodeSegment>.Enumerator _nestedEnumerator;

		/// <summary>
		/// Indicates the enumerator of <see cref="List{T}"/>. <b>This field is only used for the case that the current segment is a list.</b>
		/// </summary>
		private List<ViewNode>.Enumerator _listEnumerator;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified view.
		/// </summary>
		/// <param name="view">View.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(ValueView view) => _instance = view;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public ViewNode Current { get; private set; } = null!;

		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			ViewNode[]? array;
			List<ViewNode>? list;

#pragma warning disable format
			switch (_state)
			{
				default:
				{
					return false;
				}
				case 0:
				{
					_state = -1;
					_nestedEnumerator = _instance._viewNodeSegements.GetEnumerator();
					_state = -3;

					goto ValueCheckCore;
				}
				case 1:
				{
					_state = -3;

					goto ValueCheckCore;
				}
				case 2:
				{
					_state = -3;
					_arrayIteratedIndex++;

					goto ArrayCheck;
				}
				case 3:
				{
					_state = -4;

					goto ListEnumeratorMoveNext;
				}
				ValueCheckCore:
				{
					while (true)
					{
						if (_nestedEnumerator.MoveNext())
						{
							var a = _nestedEnumerator.Current.ActualValue;
							if (a is ViewNode node)
							{
								Current = node;
								_state = 1;

								return true;
							}

							array = a as ViewNode[];
							if (array is not null)
							{
								break;
							}

							list = a as List<ViewNode>;
							if (list is null)
							{
								continue;
							}

							goto CreateListEnumerator;
						}
						_state = -1;
						_nestedEnumerator = default;
						return false;
					}

					_array = array;
					_arrayIteratedIndex = 0;
					goto ArrayCheck;
				}
				CreateListEnumerator:
				{
					_listEnumerator = list.GetEnumerator();
					_state = -4;

					goto ListEnumeratorMoveNext;
				}
				ListEnumeratorMoveNext:
				{
					if (_listEnumerator.MoveNext())
					{
						Current = _listEnumerator.Current;
						_state = 3;
						return true;
					}

					_state = -3;
					_listEnumerator = default;

					goto ValueCheckCore;
				}
				ArrayCheck:
				{
					if (_arrayIteratedIndex < _array!.Length)
					{
						Current = _array[_arrayIteratedIndex];
						_state = 2;
						return true;
					}
					_array = null;

					goto ValueCheckCore;
				}
			}
#pragma warning restore format
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Enumerator GetEnumerator() => this;
	}

	/// <summary>
	/// Defines an enumeratot that iterates on each <see cref="ViewNodeSegment"/> instance.
	/// </summary>
	/// <seealso cref="ViewNodeSegment"/>
	public ref struct SegmentEnumerator
	{
		/// <summary>
		/// The inner enumerator.
		/// </summary>
		private LinkedList<ViewNodeSegment>.Enumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="SegmentEnumerator"/> instance via the specified segments.
		/// </summary>
		/// <param name="segments">Segments.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SegmentEnumerator(LinkedList<ViewNodeSegment> segments) => _enumerator = segments.GetEnumerator();


		/// <inheritdoc cref="IEnumerator.Current"/>
		public ViewNodeSegment Current => _enumerator.Current;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();


		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly SegmentEnumerator GetEnumerator() => this;
	}

	/// <summary>
	/// Defines an enumerator that only iterates for a fixed type of <see cref="ViewNode"/>s,
	/// specified as type argument <typeparamref name="TViewNode"/>.
	/// </summary>
	/// <typeparam name="TViewNode">The type of the node to be iterated.</typeparam>
	public ref struct OfTypeEnumerator<TViewNode> where TViewNode : ViewNode
	{
		/// <summary>
		/// The inner enumerator.
		/// </summary>
		private Enumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="OfTypeEnumerator{TViewNode}"/> instance via the specified segments.
		/// </summary>
		/// <param name="enumerator">Enumerator.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal OfTypeEnumerator(Enumerator enumerator) => _enumerator = enumerator;


		/// <summary>
		/// Indicates the current node to be iterated.
		/// </summary>
		public TViewNode Current { get; private set; } = null!;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			while (_enumerator.MoveNext())
			{
				if (_enumerator.Current is not TViewNode targetNode)
				{
					continue;
				}

				Current = targetNode;
				return true;
			}

			return false;
		}


		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OfTypeEnumerator<TViewNode> GetEnumerator() => this;
	}
}
