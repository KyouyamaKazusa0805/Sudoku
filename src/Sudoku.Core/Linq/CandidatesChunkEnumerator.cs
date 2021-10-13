namespace Sudoku.Linq;

/// <summary>
/// Defines a chunk enumerator that iterates a candidate collection of a <see cref="Grid"/>
/// through a size that defines the number of elements for each chunk as the iteration value.
/// </summary>
public unsafe ref partial struct CandidatesChunkEnumerator
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
	/// Initializes a <see cref="CandidatesChunkEnumerator"/>
	/// </summary>
	/// <param name="enumerator">The enumerator.</param>
	/// <param name="size">The size.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidatesChunkEnumerator(in Grid.CandidateCollection.Enumerator enumerator, int size)
	{
		_size = size;
		_enumerator = enumerator;
	}


	/// <summary>
	/// Gets the element in the collection at the current position of the enumerator.
	/// </summary>
	[DisallowNull]
	public int[]? Current { get; private set; } = null!;


	/// <summary>
	/// Advances the enumerator to the next element of the collection.
	/// </summary>
	/// <returns>
	/// <list type="table">
	/// <listheader>
	/// <term>Return value</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>If the enumerator was successfully advanced to the next element.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>If the enumerator has passed the end of the collection.</description>
	/// </item>
	/// </list>
	/// </returns>
	public bool MoveNext()
	{
		int[] candidateChunk = new int[_size];
		int i = 0;
		for (; i < _size; i++)
		{
			if (!_enumerator.MoveNext())
			{
				break;
			}

			candidateChunk[i] = _enumerator.Current;
		}

		if (i == 0)
		{
			return false;
		}
		else
		{
			Current = candidateChunk;
			return true;
		}
	}
}