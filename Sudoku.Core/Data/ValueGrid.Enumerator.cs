using System.Collections;
using System.Collections.Generic;

namespace Sudoku.Data
{
	public unsafe partial struct ValueGrid
	{
		/// <summary>
		/// Encapsulates an enumerator for <see cref="ValueGrid"/>.
		/// </summary>
		private struct Enumerator : IEnumerator<short>
		{
			/// <summary>
			/// Indicates the initial pointer to the first element.
			/// </summary>
			private readonly short* _pInitial;

			/// <summary>
			/// The number to iterate. If the value is 81, the grid will be end to iterate.
			/// </summary>
			private int _count;

			/// <summary>
			/// The pointer to the current element.
			/// </summary>
			/// <remarks>
			/// Please note, if the enumerator <b>must</b> points to <c>[0]</c> to <c>[80]</c>, otherwise
			/// it will cause an fatal error for memory leaking.
			/// </remarks>
			private short* _pCurrent;


			/// <summary>
			/// Initializes an instance with the specified grid.
			/// </summary>
			/// <param name="pointer">The grid.</param>
			public Enumerator(ValueGrid pointer)
			{
				_count = 0;
				_pInitial = pointer._masks;
				_pCurrent = pointer._masks;
			}


			/// <inheritdoc/>
			public readonly short Current => *_pCurrent;

			/// <inheritdoc/>
			readonly object? IEnumerator.Current => Current;


			/// <inheritdoc/>
			public void Dispose()
			{
				// Do nothing.
			}

			/// <inheritdoc/>
			public bool MoveNext()
			{
				_pCurrent++;
				return ++_count != 81;
			}

			/// <inheritdoc/>
			public void Reset()
			{
				_count = 0;
				_pCurrent = _pInitial;
			}
		}
	}
}
