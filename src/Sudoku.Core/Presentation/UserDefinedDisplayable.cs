namespace Sudoku.Presentation;

/// <summary>
/// Defines a displayable item that is user-defined.
/// </summary>
public sealed class UserDefinedDisplayable : IDisplayable, IEnumerable<ViewNode>
{
	/// <summary>
	/// Indicates the inner view.
	/// </summary>
	private readonly View _view = View.Empty;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	private Cells _cellsUsedMap = Cells.Empty;

	/// <summary>
	/// Indicates the candidates used.
	/// </summary>
	private Candidates _candidatesUsedMap = Candidates.Empty;


	/// <inheritdoc/>
	ImmutableArray<Conclusion> IDisplayable.Conclusions => ImmutableArray<Conclusion>.Empty;

	/// <inheritdoc/>
	ImmutableArray<View> IDisplayable.Views => ImmutableArray.Create(_view);


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

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public List<ViewNode>.Enumerator GetEnumerator() => _view.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<ViewNode> IEnumerable<ViewNode>.GetEnumerator() => GetEnumerator();
}
