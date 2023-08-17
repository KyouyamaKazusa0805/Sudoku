namespace Sudoku.Linq;

partial struct CandidateMapGroup<TKey, TValue>
{
	/// <summary>
	/// Indicates the enumerator type.
	/// </summary>
	/// <param name="values">The values to be assigned.</param>
	public ref struct Enumerator(TValue[] values)
	{
		/// <summary>
		/// Indicates the total number of elements.
		/// </summary>
		private readonly int _totalCount = values.Length;

		/// <summary>
		/// Indicates the internal values.
		/// </summary>
		private readonly TValue[] _values = values;

		/// <summary>
		/// Indicates the current index.
		/// </summary>
		private int _currentIndex = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly ref TValue Current => ref _values[_currentIndex];


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_currentIndex < _totalCount;
	}
}
