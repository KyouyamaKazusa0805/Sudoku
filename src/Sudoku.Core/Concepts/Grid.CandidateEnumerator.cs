namespace Sudoku.Concepts;

partial struct Grid
{
	/// <summary>
	/// Defines the default enumerator that iterates the <see cref="Grid"/> through the candidates in the current <see cref="Grid"/> instance.
	/// </summary>
	/// <param name="arr">The reference to an array.</param>
	/// <see cref="Grid"/>
	public ref struct CandidateEnumerator([UnscopedRef] ref Mask arr)
	{
		/// <summary>
		/// The pointer to the start value.
		/// </summary>
		private readonly ref Mask _start = ref SubtractByteOffset(ref arr, 1);

		/// <summary>
		/// The current pointer.
		/// </summary>
		private ref Mask _refCurrent = ref SubtractByteOffset(ref arr, 1);

		/// <summary>
		/// Indicates the current mask.
		/// </summary>
		private Mask _currentMask;

		/// <summary>
		/// The current index.
		/// </summary>
		private int _currentIndex = -1;


		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		public readonly Candidate Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _currentIndex * 9 + TrailingZeroCount(_currentMask);
		}

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
			if (_currentMask != 0 && (_currentMask &= (Mask)~(1 << TrailingZeroCount(_currentMask))) != 0)
			{
				return true;
			}

			while (_currentIndex < 81)
			{
				_currentIndex++;
				if (MaskToStatus(AddByteOffset(ref _start, _currentIndex)) == CellStatus.Empty)
				{
					break;
				}
			}

			if (_currentIndex == 81)
			{
				return false;
			}

			_refCurrent = ref AddByteOffset(ref _start, _currentIndex + 1);
			_currentMask = (Mask)(_refCurrent & MaxCandidatesMask);

			return true;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly CandidateEnumerator GetEnumerator() => this;
	}
}
