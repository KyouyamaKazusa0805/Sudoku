namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// Provides a grouped iterator that is used in
	/// <see langword="skip"/> clause (only in Visual Basic .NET) in LINQ.
	/// </summary>
	internal sealed class IteratorSkipIterator<T> : IIterator<T>
	{
		/// <summary>
		/// Indicates the enumerator.
		/// </summary>
		private readonly IIterator<T> _enumerator;


		/// <summary>
		/// Initializes an instance with the enumerator and the count.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		/// <param name="count">The count.</param>
		public IteratorSkipIterator(IIterator<T> enumerator, int count)
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
		public T Current => _enumerator.Current;


		/// <inheritdoc/>
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
