using static Sudoku.Data.SudokuGrid;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// Provides a grouped iterator that is used in
	/// <see langword="skip"/> clause (only in Visual Basic .NET) in LINQ.
	/// </summary>
	internal sealed class SkipIterator : IIterator<int>
	{
		/// <summary>
		/// Indicates the enumerator.
		/// </summary>
		private readonly Enumerator _enumerator;


		/// <summary>
		/// Initializes an instance with the enumerator and the count.
		/// </summary>
		/// <param name="enumerator">(<see langword="in"/> parameter) The enumerator.</param>
		/// <param name="count">The count.</param>
		public SkipIterator(in Enumerator enumerator, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (!enumerator.MoveNext())
				{
					break;
				}
			}

			_enumerator = enumerator;
		}


		/// <inheritdoc/>
		public int Current => _enumerator.Current;


		/// <inheritdoc/>
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
