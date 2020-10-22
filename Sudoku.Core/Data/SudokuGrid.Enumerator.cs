using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// The inner enumerator.
		/// </summary>
		private unsafe struct Enumerator : IEnumerator<short>
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
			/// The current index.
			/// </summary>
			private int _currentIndex;


			/// <summary>
			/// Initializes an instance with the specified pointer to an array to iterate.
			/// </summary>
			/// <param name="arr">The pointer to an array.</param>
			public Enumerator(short* arr)
			{
				_start = arr;
				_currentPointer = arr;
				_currentIndex = 0;
			}


			/// <inheritdoc/>
			public short Current => *_currentPointer;

			/// <inheritdoc/>
			object? IEnumerator.Current => Current;


			/// <inheritdoc/>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose() { }

			/// <inheritdoc/>
			public bool MoveNext()
			{
				_currentPointer++;
				return ++_currentIndex != 81;
			}

			/// <inheritdoc/>
			public void Reset()
			{
				_currentPointer = _start;
				_currentIndex = 0;
			}
		}
	}
}
