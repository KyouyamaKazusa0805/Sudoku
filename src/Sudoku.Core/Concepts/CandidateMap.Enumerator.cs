namespace Sudoku.Concepts;

partial struct CandidateMap
{
	/// <summary>
	/// Represents an enumerator type that iterates on each candidate offsets.
	/// </summary>
	/// <param name="offsets">Indicates the offsets.</param>
	public struct Enumerator(Candidate[] offsets) : IEnumerator<Candidate>
	{
		/// <summary>
		/// The internal field for offsets.
		/// </summary>
		private readonly Candidate[] _offsets = offsets;

		/// <summary>
		/// Indicates the index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc/>
		public readonly Candidate Current => _offsets[_index];

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc/>
		public bool MoveNext() => ++_index < _offsets.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
