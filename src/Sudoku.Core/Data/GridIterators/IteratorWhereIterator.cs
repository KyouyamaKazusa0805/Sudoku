using System;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// The iterator that used in the <see langword="where"/> clause in LINQ.
	/// </summary>
	/// <typeparam name="TResult">The type of the target elements.</typeparam>
	internal sealed class IteratorWhereIterator<TResult> : IIterator<TResult>
	{
		/// <summary>
		/// The enumerator that iterates on all elements.
		/// </summary>
		private readonly IIterator<TResult> _iterator;

		/// <summary>
		/// Indicates the condition.
		/// </summary>
		private readonly Predicate<TResult> _condition;


		/// <summary>
		/// Initializes an instance with the iterator and the condition method.
		/// </summary>
		/// <param name="iterator">The iterator.</param>
		/// <param name="condition">The condition method.</param>
		public IteratorWhereIterator(IIterator<TResult> iterator, Predicate<TResult> condition)
		{
			_iterator = iterator;
			_condition = condition;
		}


		/// <inheritdoc/>
		[NotNull]
		public TResult? Current { get; private set; }


		/// <inheritdoc/>
		public bool MoveNext()
		{
			while (_iterator.MoveNext())
			{
				if (_condition(Current = _iterator.Current))
				{
					return true;
				}
			}

			return false;
		}
	}
}
