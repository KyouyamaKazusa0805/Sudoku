namespace Sudoku.Linq;

/// <summary>
/// The enumerable collection that iterates for a <see cref="CandidatesDistinctEnumerator"/> instance.
/// </summary>
/// <seealso cref="CandidatesDistinctEnumerator"/>
public readonly unsafe ref partial struct CandidatesDistinctEnumerable
{
	/// <summary>
	/// Indicates the pointer to a grid.
	/// </summary>
	[NotNull, DisallowNull]
	private readonly Grid* _gridHandle;

	/// <summary>
	/// Indicates the key selector method.
	/// </summary>
	[NotNull, DisallowNull]
	private readonly delegate*<int, int, bool> _keySelector;


	/// <summary>
	/// Initializes a <see cref="CandidatesDistinctEnumerable"/> instance
	/// using the specified <see cref="Grid"/> and the key selector method.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="selector">The key selector.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidatesDistinctEnumerable(in Grid grid, [NotNull, DisallowNull] delegate*<int, int, bool> selector)
	{
		fixed (Grid* gridHandle = &grid)
		{
			_gridHandle = gridHandle;
		}
		_keySelector = selector;
	}

	/// <summary>
	/// Gets the enumerator that iterates on candidate distincted by the specified method.
	/// </summary>
	/// <returns>The enumerator of candidate distincted by the specified method..</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidatesDistinctEnumerator GetEnumerator() => new(_gridHandle, _keySelector);
}
