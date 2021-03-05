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
		/// Groups the elements of a sequence according to a specified key selector function.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="keySelector">The key selecting method.</param>
		/// <returns>
		/// An <see cref="IIterator{T}"/> of <see cref="IGroup{TKey, TValue}"/> where each
		/// object contains a sequence of objects and a key.
		/// </returns>
		public IIterator<IGroup<TKey, int>> GroupBy<TKey>(Func<int, TKey> keySelector) where TKey : notnull =>
			new GroupedIterator<TKey>(GetEnumerator(), keySelector);

		/// <summary>
		/// Groups the elements of a sequence according to a specified key selector function.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TConverted">The type of the value converted.</typeparam>
		/// <param name="keySelector">The key selecting method.</param>
		/// <returns>
		/// An <see cref="IIterator{T}"/> of <see cref="IGroup{TKey, TValue}"/> where each
		/// object contains a sequence of objects and a key.
		/// </returns>
		public IIterator<IGroup<TKey, TConverted>> GroupBy<TKey, TConverted>(
			Func<int, TKey> keySelector, Func<int, TConverted> elementSelector) where TKey : notnull =>
			new GroupedIterator<TKey, TConverted>(GetEnumerator(), keySelector, elementSelector);

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

		/// <summary>
		/// To skip the several elements in this collection.
		/// </summary>
		/// <param name="count">The number of elements you want to skip.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public IIterator<int> Skip(int count) => new SkipIterator(GetEnumerator(), count);

		/// <summary>
		/// To take the several elements in this collection.
		/// </summary>
		/// <param name="count">The number of elements you want to take.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public IEnumerable<int> Take(int count)
		{
			var enumerator = GetEnumerator();
			for (int i = 0; i < count; i++)
			{
				if (!enumerator.MoveNext())
				{
					yield break;
				}

				yield return enumerator.Current;
			}
		}
	}
}
