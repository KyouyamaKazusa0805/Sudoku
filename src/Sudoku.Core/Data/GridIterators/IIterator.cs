#if SUDOKU_GRID_LINQ

#pragma warning disable CA1816

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// Define an enumerator for sudoku grid only.
	/// </summary>
	/// <typeparam name="T">The target type of the iteration element.</typeparam>
	public partial interface IIterator<T> : IEnumerator<T>, IEnumerable<T>
	{
		/// <inheritdoc/>
		object? IEnumerator.Current => Current;


		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

		/// <inheritdoc cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <inheritdoc/>
		void IDisposable.Dispose()
		{
		}

		/// <inheritdoc/>
		void IEnumerator.Reset()
		{
		}
	}
}

#endif