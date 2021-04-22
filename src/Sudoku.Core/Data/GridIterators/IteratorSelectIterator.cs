#if SUDOKU_GRID_LINQ

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// The iterator that used in the <see langword="select"/> clause in LINQ.
	/// </summary>
	/// <typeparam name="T">The type of the base elements.</typeparam>
	/// <typeparam name="TResult">The type of the target elements.</typeparam>
	internal sealed class IteratorSelectIterator<T, TResult> : IIterator<TResult>
	{
		/// <summary>
		/// The enumerator that iterates on all elements.
		/// </summary>
		private readonly IIterator<T> _iterator;

		/// <summary>
		/// Indicates the convert method.
		/// </summary>
		private readonly Func<T, TResult> _converter;


		/// <summary>
		/// Initializes an instance with the enumerator and the convert method.
		/// </summary>
		/// <param name="iterator">The iterator.</param>
		/// <param name="converter">The convert method.</param>
		public IteratorSelectIterator(IIterator<T> iterator, Func<T, TResult> converter)
		{
			_iterator = iterator;
			_converter = converter;
		}


		/// <inheritdoc/>
		[NotNull]
		public TResult? Current { get; private set; }


		/// <inheritdoc/>
		public bool MoveNext()
		{
			if (_iterator.MoveNext())
			{
				Current = _converter(_iterator.Current);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

#endif