using System.Runtime.CompilerServices;
using Sudoku.Data.Extensions;
using static System.Numerics.BitOperations;

namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// The inner enumerator.
		/// </summary>
		public unsafe ref partial struct Enumerator
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
			private short _currentMask;

			/// <summary>
			/// The current index.
			/// </summary>
			private int _currentIndex;


			/// <summary>
			/// Initializes an instance with the specified pointer to an array to iterate.
			/// </summary>
			/// <param name="arr">The pointer to an array.</param>
			/// <remarks>
			/// Note here we should point at the one-unit-lengthed memory before the array start.
			/// </remarks>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(short* arr) : this()
			{
				_currentPointer = _start = arr - 1;
				_currentIndex = -1;
			}


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
			/// <see langword="true"/> if the enumerator was successfully advanced to the next element;
			/// <see langword="false"/> if the enumerator has passed the end of the collection.
			/// </returns>
			public bool MoveNext()
			{
				if (_currentMask == 0)
				{
					goto MovePointer;
				}
				else
				{
					_currentMask &= (short)~(1 << TrailingZeroCount(_currentMask));
					if (_currentMask == 0)
					{
						goto MovePointer;
					}

					return true;
				}

			MovePointer:
				do _currentIndex++; while (
					_start[_currentIndex + 1].MaskToStatus() != CellStatus.Empty
					&& _currentIndex != Length + 1
				);

				if (_currentIndex == Length + 1)
				{
					return false;
				}

				_currentPointer = _start + _currentIndex + 1;
				_currentMask = (short)(*_currentPointer & MaxCandidatesMask);
				return true;
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
		}
	}
}
