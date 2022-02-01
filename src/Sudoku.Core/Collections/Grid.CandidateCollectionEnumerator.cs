using static System.Numerics.BitOperations;

namespace Sudoku.Collections;

partial struct Grid
{
	/// <summary>
	/// Defines the default enumerator that iterates the <see cref="Grid"/>
	/// through the candidates in the current <see cref="Grid"/> instance.
	/// </summary>
	public unsafe ref partial struct CandidateCollectionEnumerator
	{
		/// <summary>
		/// The pointer to the start value.
		/// </summary>
		private readonly short* _start;


		/// <summary>
		/// The current pointer.
		/// </summary>
		private short* _currentPointer;

		/// <summary>
		/// Indicates the current mask.
		/// </summary>
		private short _currentMask = 0;

		/// <summary>
		/// The current index.
		/// </summary>
		private int _currentIndex = -1;


		/// <summary>
		/// Initializes an instance with the specified pointer to an array to iterate.
		/// </summary>
		/// <param name="arr">The pointer to an array.</param>
		/// <remarks>
		/// Note here we should point at the one-unit-lengthed memory before the array start.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CandidateCollectionEnumerator(short* arr) => _currentPointer = _start = arr - 1;


		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		public readonly int Current
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
			if (_currentMask == 0 || (_currentMask &= (short)~(1 << TrailingZeroCount(_currentMask))) == 0)
			{
				goto MovePointer;
			}
			else
			{
				goto ReturnTrue;
			}

		MovePointer:
			do
			{
				_currentIndex++;
			}
			while (MaskToStatus(_start[_currentIndex + 1]) != CellStatus.Empty && _currentIndex != 81 + 1);

			if (_currentIndex == 81 + 1)
			{
				goto ReturnFalse;
			}

			_currentPointer = _start + _currentIndex + 1;
			_currentMask = (short)(*_currentPointer & MaxCandidatesMask);

		ReturnTrue:
			return true;

		ReturnFalse:
			return false;
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element
		/// in the collection.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_currentPointer = _start;
			_currentIndex = -1;
			_currentMask = default;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly CandidateCollectionEnumerator GetEnumerator() => this;
	}
}
