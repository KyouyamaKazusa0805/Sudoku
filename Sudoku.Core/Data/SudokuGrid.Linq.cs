using System;
using System.Collections.Generic;
using Sudoku.Data.GridIterators;

namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// Converts all elements to the target instances using the specified converter.
		/// </summary>
		/// <typeparam name="TConverted">The type of target element.</typeparam>
		/// <param name="converter">The converter.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public IIterator<TConverted> Select<TConverted>(Func<int, TConverted> converter) =>
			new SelectIterator<TConverted>(GetEnumerator(), converter);

		/// <summary>
		/// Checks all elements, and selects the values that satisfy the condition
		/// specified as a delegate method.
		/// </summary>
		/// <param name="condition">The condition method.</param>
		/// <returns>The iterator that iterates on each target element satisfying the condition.</returns>
		public IIterator<int> Where(Predicate<int> condition) => new WhereIterator(GetEnumerator(), condition);

		/// <summary>
		/// Projects each element of a sequence to an <see cref="IEnumerable{T}"/>,
		/// flattens the resulting sequences into one sequence, and invokes a result selector function
		/// on each element therein.
		/// </summary>
		/// <typeparam name="T">The first type of the base elements.</typeparam>
		/// <typeparam name="TResult">The type of the projection result.</typeparam>
		/// <param name="collectionSelector">The collection selector.</param>
		/// <param name="resultSelector">The result selector.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public IEnumerable<TResult> SelectMany<T, TResult>(
			Func<int, IEnumerable<T>> collectionSelector, Func<int, T, TResult> resultSelector)
		{
			foreach (int element in this)
			{
				foreach (var subElement in collectionSelector(element))
				{
					yield return resultSelector(element, subElement);
				}
			}
		}
	}
}
