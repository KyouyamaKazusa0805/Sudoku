using System;
using System.Collections;
using System.Collections.Generic;

namespace Sudoku.Extensions
{
	public static partial class RangeEx
	{
		/// <summary>
		/// Defines a range iterator.
		/// </summary>
		private struct RangeIterator : IEnumerator<int>
		{
			/// <summary>
			/// Indicates the end index, read-only.
			/// </summary>
			private readonly int _end;

			/// <summary>
			/// Indicates the current value.
			/// </summary>
			private int _current;


			/// <inheritdoc/>
			readonly int IEnumerator<int>.Current => _current;

			/// <inheritdoc/>
			readonly object? IEnumerator.Current => _current;


			/// <summary>
			/// Initializes an instance with start and end index value.
			/// </summary>
			/// <param name="start">The start index value.</param>
			/// <param name="end">The end index value.</param>
			public RangeIterator(int start, int end) => (_current, _end) = (start, end);


			/// <inheritdoc/>
			bool IEnumerator.MoveNext() => ++_current != _end;

			/// <inheritdoc/>
			void IDisposable.Dispose() { }

			/// <inheritdoc/>
			readonly void IEnumerator.Reset() { }
		}
	}
}
