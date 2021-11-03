namespace Sudoku.Linq;

/// <summary>
/// Defines an enumerator that iterates the candidates that filtered and distincted by the specified method.
/// </summary>
public unsafe ref partial struct CandidatesDistinctEnumerator
{
	/// <summary>
	/// Indicates the inner buffer.
	/// </summary>
	private readonly int[] _buffer;

	/// <summary>
	/// Indicates the enumerator to iterate all candidates.
	/// </summary>
	private readonly Grid.CandidateCollection.Enumerator _enumerator;

	/// <summary>
	/// Indicates the key selector method.
	/// </summary>
	private readonly delegate*<int, int, bool> _keySelector;


	/// <summary>
	/// Indicates the value that records how many times the enumerator iterates.
	/// </summary>
	private int _i;


	/// <summary>
	/// Initializes a <see cref="CandidatesDistinctEnumerator"/> instance
	/// using the specified enumerator and the key selector method.
	/// </summary>
	/// <param name="gridHandle">The pointer to a grid.</param>
	/// <param name="keySelector">The key selector.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidatesDistinctEnumerator(Grid* gridHandle, delegate*<int, int, bool> keySelector)
	{
		_enumerator = gridHandle->Candidates.GetEnumerator();
		_keySelector = keySelector;
		_buffer = new int[gridHandle->CandidatesCount];
		_i = -1;
	}


	/// <summary>
	/// Gets the element in the collection at the current position of the enumerator.
	/// </summary>
	public int Current { get; private set; } = -1;


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
		int c;
		if (++_i == 0)
		{
			if (!_enumerator.MoveNext())
			{
				goto ReturnFalse;
			}

			c = _enumerator.Current;

			goto ReturnTrue;
		}

		while (_enumerator.MoveNext())
		{
			bool isDuplicated = false;

			c = _enumerator.Current;
			for (int index = 0; index < _i; index++)
			{
				if (_keySelector(_buffer[index], c))
				{
					isDuplicated = true;
					break;
				}
			}

			if (!isDuplicated)
			{
				goto ReturnTrue;
			}
		}

	ReturnFalse:
		return false;

	ReturnTrue:
		Current = c;
		_buffer[_i] = c;
		return true;
	}
}