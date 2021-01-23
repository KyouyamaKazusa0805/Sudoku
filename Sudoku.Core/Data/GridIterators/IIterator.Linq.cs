using System;

namespace Sudoku.Data.GridIterators
{
	partial interface IIterator<T>
	{
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

		/// <summary>
		/// Groups the elements of a sequence according to a specified key selector function.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="keySelector">The key selecting method.</param>
		/// <returns>
		/// An <see cref="IIterator{T}"/> of <see cref="IGroup{TKey, TValue}"/> where each
		/// object contains a sequence of objects and a key.
		/// </returns>
		public IIterator<IGroup<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector) where TKey : notnull =>
			new IteratorGroupedIterator<TKey, T>(this, keySelector);

		/// <summary>
		/// Groups the elements of a sequence according to a specified key selector function,
		/// and a convert method.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TConverted">The type of the value converted.</typeparam>
		/// <param name="keySelector">The key selecting method.</param>
		/// <param name="converter">The convert method.</param>
		/// <returns>
		/// An <see cref="IIterator{T}"/> of <see cref="IGroup{TKey, TValue}"/> where each
		/// object contains a sequence of objects and a key.
		/// </returns>
		public IIterator<IGroup<TKey, TConverted>> GroupBy<TKey, TConverted>(
			Func<T, TKey> keySelector, Func<T, TConverted> converter) where TKey : notnull =>
			new IteratorGroupedIterator<TKey, T, TConverted>(this, keySelector, converter);

		/// <summary>
		/// To skip the several elements in this collection.
		/// </summary>
		/// <param name="count">The number of elements you want to skip.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public IIterator<T> Skip(int count) => new IteratorSkipIterator<T>(this, count);
	}
}
