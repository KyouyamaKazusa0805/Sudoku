namespace Sudoku.Runtime.MeasuringServices;

public partial struct UndirectedCellGraph
{
	/// <summary>
	/// Represents an enumerator type that iterates on each cell offsets.
	/// </summary>
	/// <param name="_offset">Indicates the offsets.</param>
	public ref struct Enumerator(Cell[] _offset) : IEnumerator<Cell>
	{
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
