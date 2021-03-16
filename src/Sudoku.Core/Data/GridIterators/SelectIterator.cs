using System;
using System.Diagnostics.CodeAnalysis;
using static Sudoku.Data.SudokuGrid;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// The iterator that used in the <see langword="select"/> clause in LINQ.
	/// </summary>
	/// <typeparam name="T">The type of the target elements.</typeparam>
	internal sealed class SelectIterator<T> : IIterator<T>
	{
		/// <summary>
		/// The enumerator that iterates on all elements.
		/// </summary>
		private readonly Enumerator _enumerator;

		/// <summary>
		/// Indicates the convert method.
		/// </summary>
		private readonly Func<int, T> _converter;


		/// <summary>
		/// Initializes an instance with the enumerator and the convert method.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		/// <param name="converter">The convert method.</param>
		public SelectIterator(in Enumerator enumerator, Func<int, T> converter)
		{
			_enumerator = enumerator;
			_converter = converter;
		}


		/// <inheritdoc/>
		[NotNull]
		public T? Current { get; private set; }


		/// <inheritdoc/>
		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				Current = _converter(_enumerator.Current);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
