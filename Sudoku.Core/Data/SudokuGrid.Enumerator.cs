using System;
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
		public unsafe struct Enumerator : IEnumerator<short>
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
			[CLSCompliant(false)]
			public Enumerator(short* arr)
			{
				// Note here we should point at the one-unit-lengthed memory before the array start.
				_currentPointer = _start = arr - 1;
				_currentIndex = 0;
			}


			/// <inheritdoc/>
			public readonly short Current => *_currentPointer;

			/// <inheritdoc/>
			readonly object? IEnumerator.Current => Current;


			/// <inheritdoc/>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose() { }

			/// <inheritdoc/>
			public bool MoveNext()
			{
				_currentPointer++;
				return ++_currentIndex != 82;
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
