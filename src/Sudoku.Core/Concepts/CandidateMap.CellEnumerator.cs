namespace Sudoku.Concepts;

public partial struct CandidateMap
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each cell of the collection.
	/// </summary>
	/// <param name="_candidates">Indicates the candidate offsets.</param>
	public ref struct CellEnumerator(Candidate[] _candidates) : IEnumerator<Cell>
	{
		/// <summary>
		/// Indicates the index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc/>
		public readonly Cell Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _candidates[_index] / 9;
		}

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <summary>
		/// Returns itself, in order to iterate the value using <see langword="foreach"/> loop.
		/// </summary>
		/// <returns>The enumerator itself.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly CellEnumerator GetEnumerator() => this;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _candidates.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
