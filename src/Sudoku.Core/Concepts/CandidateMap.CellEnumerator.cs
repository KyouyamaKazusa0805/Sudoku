namespace Sudoku.Concepts;

public partial struct CandidateMap
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each cell of the collection.
	/// </summary>
	/// <param name="candidates">Indicates the candidate offsets.</param>
	public ref struct CellEnumerator(Candidate[] candidates)
	{
		/// <summary>
		/// Indicates the index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly Cell Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => candidates[_index] / 9;
		}


		/// <summary>
		/// Returns itself, in order to iterate the value using <see langword="foreach"/> loop.
		/// </summary>
		/// <returns>The enumerator itself.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly CellEnumerator GetEnumerator() => this;

		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < candidates.Length;
	}
}
