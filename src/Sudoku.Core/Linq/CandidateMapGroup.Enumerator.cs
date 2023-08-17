namespace Sudoku.Linq;

partial struct CandidateMapGroup<TKey>
{
	/// <summary>
	/// Indicates the enumerator type.
	/// </summary>
	/// <param name="map">The candidate map to be assigned.</param>
	public ref struct Enumerator(scoped in CandidateMap map)
	{
		/// <summary>
		/// Indicates the total number of elements.
		/// </summary>
		private readonly int _totalCount = map.Count;

		/// <summary>
		/// Indicates the internal values.
		/// </summary>
		private readonly Candidate[] _values = [.. map];

		/// <summary>
		/// Indicates the current index.
		/// </summary>
		private int _currentIndex = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly Candidate Current => _values[_currentIndex];


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_currentIndex < _totalCount;
	}
}
