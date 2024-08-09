namespace Sudoku.Concepts;

public partial struct CandidateMap
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each candidate of the collection,
	/// with its cell and digit value in the target tuple.
	/// </summary>
	/// <param name="_candidates">Indicates the candidate offsets.</param>
	public ref struct CellDigitEnumerator(Candidate[] _candidates) : IEnumerator<(Candidate Candidate, Cell Cell, Digit Digit)>
	{
		/// <summary>
		/// Indicates the index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly (Candidate Candidate, Cell Cell, Digit Digit) Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var candidate = _candidates[_index];
				return (candidate, candidate / 9, candidate % 9);
			}
		}

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <summary>
		/// Returns itself, in order to iterate the value using <see langword="foreach"/> loop.
		/// </summary>
		/// <returns>The enumerator itself.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly CellDigitEnumerator GetEnumerator() => this;

		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _candidates.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
