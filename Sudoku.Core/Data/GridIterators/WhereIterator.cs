using System;
using static Sudoku.Data.SudokuGrid;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// The iterator that used in the <see langword="where"/> clause in LINQ.
	/// </summary>
	internal sealed class WhereIterator : IIterator<int>
	{
		/// <summary>
		/// The enumerator that iterates on all elements.
		/// </summary>
		private readonly Enumerator _enumerator;

		/// <summary>
		/// Indicates the condition.
		/// </summary>
		private readonly Predicate<int> _condition;


		/// <summary>
		/// Initializes an instance with the enumerator and the condition method.
		/// </summary>
		/// <param name="enumerator">(<see langword="in"/> parameter) The enumerator.</param>
		/// <param name="condition">The condition method.</param>
		public WhereIterator(in Enumerator enumerator, Predicate<int> condition)
		{
			_enumerator = enumerator;
			_condition = condition;
		}


		/// <inheritdoc/>
		public int Current { get; private set; }


		/// <inheritdoc/>
		public bool MoveNext()
		{
			while (_enumerator.MoveNext())
			{
				if (_condition(Current = _enumerator.Current))
				{
					return true;
				}
			}

			return false;
		}
	}
}
