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
	public interface IIterator<T> : IEnumerator<T>, IEnumerable<T>
	{
		/// <inheritdoc/>
		object? IEnumerator.Current => Current;


		/// <summary>
		/// Converts all elements to the target instances using the specified converter.
		/// </summary>
		/// <typeparam name="TResult">The type of target element.</typeparam>
		/// <param name="converter">The converter.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public IIterator<TResult> Select<TResult>(Func<T, TResult> converter) =>
			new IteratorSelectIterator<T, TResult>(this, converter);

		/// <summary>
		/// Checks all elements, and selects the values that satisfy the condition
		/// specified as a delegate method.
		/// </summary>
		/// <param name="condition">The condition method.</param>
		/// <returns>The iterator that iterates on each target element satisfying the condition.</returns>
		public IIterator<T> Where(Predicate<T> condition) => new IteratorWhereIterator<T>(this, condition);

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
