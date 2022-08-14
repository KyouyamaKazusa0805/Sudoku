namespace Sudoku.Presentation;

/// <summary>
/// Defines a displayable item that is user-defined.
/// </summary>
public sealed class UserDefinedDisplayable :
	ICloneable,
	IDisplayable,
	IEnumerable<ViewNode>,
	IReadOnlyCollection<ViewNode>,
	IReadOnlyList<ViewNode>
{
	/// <summary>
	/// Indicates the inner view.
	/// </summary>
	[JsonIgnore]
	private readonly View _view = View.Empty;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	[JsonIgnore]
	private Cells _cellsUsedMap = Cells.Empty;

	/// <summary>
	/// Indicates the candidates used.
	/// </summary>
	[JsonIgnore]
	private Candidates _candidatesUsedMap = Candidates.Empty;


	/// <summary>
	/// Initializes a <see cref="UserDefinedDisplayable"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UserDefinedDisplayable()
	{
	}

	/// <summary>
	/// Initializes a <see cref="UserDefinedDisplayable"/> instance via the specified view nodes.
	/// </summary>
	/// <param name="viewNodes">A list of view nodes.</param>
	private UserDefinedDisplayable(IEnumerable<ViewNode> viewNodes)
	{
		foreach (var node in viewNodes)
		{
			AddRemove(node);
		}
	}


	/// <summary>
	/// Indicates the number of elements stored in this collection.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _view.Count;
	}

	/// <summary>
	/// Indicates the cell nodes.
	/// </summary>
	public CellViewNode[] CellNodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _view.CellNodes.ToArray();

		[Obsolete("This setter is reserved by compiler.", true)]
		set
		{
			foreach (var node in value)
			{
				AddRemove(node);
			}
		}
	}

	/// <summary>
	/// Indicates the candidate nodes.
	/// </summary>
	public CandidateViewNode[] CandidateNodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _view.CandidateNodes.ToArray();

		[Obsolete("This setter is reserved by compiler.", true)]
		set
		{
			foreach (var node in value)
			{
				AddRemove(node);
			}
		}
	}

	/// <inheritdoc/>
	ImmutableArray<Conclusion> IDisplayable.Conclusions => ImmutableArray<Conclusion>.Empty;

	/// <inheritdoc/>
	ImmutableArray<View> IDisplayable.Views => ImmutableArray.Create(_view);


	/// <summary>
	/// Gets the view node at the specified index stored in this collection.
	/// </summary>
	/// <param name="index">The index that you want to get.</param>
	/// <returns>The view node at the specified index.</returns>
	public ViewNode this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _view[index];
	}


	/// <summary>
	/// Adds a view node into the current collection, with the specified behavior
	/// to control the duplicate items.
	/// </summary>
	/// <param name="viewNode">The view node to be added.</param>
	/// <param name="behavior">The behavior on determining the duplicate items added.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="behavior"/> is not defined in the enumeration type.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(ViewNode viewNode, ViewNodeAddBehavior behavior)
		=> (
			(Action<ViewNode>)(
				behavior switch
				{
					ViewNodeAddBehavior.ForceAdd => ForceAdd,
					ViewNodeAddBehavior.ReplaceIfDuplicate => AddRemove,
					_ => throw new ArgumentOutOfRangeException(nameof(behavior))
				}
			)
		)(viewNode);

	/// <summary>
	/// Adds the view node into the collection no matter what status the collection is.
	/// </summary>
	/// <param name="viewNode">The view node to be added.</param>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="viewNode"/> is not <see cref="CellViewNode"/>
	/// or <see cref="CandidateViewNode"/>.
	/// </exception>
	public void ForceAdd(ViewNode viewNode)
	{
		switch (viewNode)
		{
			case CellViewNode { Cell: var c }:
			{
				_cellsUsedMap.Add(c);
				_view.Add(viewNode);

				break;
			}
			case CandidateViewNode { Candidate: var c }:
			{
				_candidatesUsedMap.Add(c);
				_view.Add(viewNode);

				break;
			}
			default:
			{
				throw new NotSupportedException("The specified node kind is not supported at present.");
			}
		}
	}

	/// <summary>
	/// Adds or removes the view node. If the node exists in the collection, remove it;
	/// otherwise, add it.
	/// </summary>
	/// <param name="viewNode">The view node to be removed or added.</param>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="viewNode"/> is not <see cref="CellViewNode"/>
	/// or <see cref="CandidateViewNode"/>.
	/// </exception>
	public void AddRemove(ViewNode viewNode)
	{
		switch (viewNode)
		{
			case CellViewNode { Cell: var c, Identifier: var i }:
			{
				if (_cellsUsedMap.Contains(c))
				{
					bool predicate(ViewNode n) => n is CellViewNode { Cell: var c2 } && c == c2;
					var foundNode = _view.Contains(predicate);
					if (foundNode is not null)
					{
						_cellsUsedMap.Remove(c);
						_view.Remove(foundNode);
					}
					else
					{
						_view.Add(viewNode);
					}
				}
				else
				{
					_cellsUsedMap.Add(c);
					_view.Add(viewNode);
				}

				break;
			}
			case CandidateViewNode { Candidate: var c, Identifier: var i }:
			{
				if (_candidatesUsedMap.Contains(c))
				{
					bool predicate(ViewNode n) => n is CandidateViewNode { Candidate: var c2 } && c == c2;
					var foundNode = _view.Contains(predicate);
					if (foundNode is not null)
					{
						_candidatesUsedMap.Remove(c);
						_view.Remove(foundNode);
					}
					else
					{
						_view.Add(viewNode);
					}
				}
				else
				{
					_candidatesUsedMap.Add(c);
					_view.Add(viewNode);
				}

				break;
			}
			default:
			{
				throw new NotSupportedException("The specified node kind is not supported at present.");
			}
		}
	}

	/// <inheritdoc cref="ICloneable.Clone"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UserDefinedDisplayable Clone() => new(_view);

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public List<ViewNode>.Enumerator GetEnumerator() => _view.GetEnumerator();

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
