namespace Sudoku.Linq;

/// <summary>
/// The enumerable collection that iterates for a <see cref="CandidatesChunkEnumerator"/> instance.
/// </summary>
/// <seealso cref="CandidatesChunkEnumerator"/>
public readonly ref partial struct CandidatesChunkEnumerable
{
	/// <summary>
	/// Indicates the size of the each chuck.
	/// </summary>
	private readonly int _size;

	/// <summary>
	/// Indicates the enumerator.
	/// </summary>
	private readonly Grid.CandidateCollection.Enumerator _enumerator;


	/// <summary>
	/// Initializes a <see cref="CandidatesChunkEnumerator"/> instance using the specified enumerator
	/// of the sudoku grid, and the size.
	/// </summary>
	/// <param name="enumerator">The enumerator.</param>
	/// <param name="size">The size.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidatesChunkEnumerable(in Grid.CandidateCollection.Enumerator enumerator, int size)
	{
		_size = size;
		_enumerator = enumerator;
	}


	/// <summary>
	/// Gets the enumerator that iterates on candidate chunks.
	/// </summary>
	/// <returns>The enumerator of candidate chunks.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidatesChunkEnumerator GetEnumerator() => new(_enumerator, _size);
}
