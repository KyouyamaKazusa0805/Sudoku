namespace Sudoku.Concepts;

partial struct CellMap
{
	/// <summary>
	/// Represents an enumerator type that iterates on each cell offsets.
	/// </summary>
	/// <param name="offset">Indicates the offsets.</param>
	public struct Enumerator(Cell[] offset) : IEnumerator<Cell>
	{
		/// <summary>
		/// The internal field for offsets.
		/// </summary>
		private readonly Cell[] _offset = offset;

		/// <summary>
		/// Indicates the index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc/>
		public readonly Cell Current => _offset[_index];

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc/>
		public bool MoveNext() => ++_index < _offset.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
