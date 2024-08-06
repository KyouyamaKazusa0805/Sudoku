namespace Sudoku.Drawing;

public partial struct DashArray
{
	/// <summary>
	/// Defines an enumerator of this type.
	/// </summary>
	/// <param name="_doubles">The double values.</param>
	public ref struct Enumerator(List<double> _doubles) : IEnumerator<double>
	{
		/// <summary>
		/// Indicates the index of the current position.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly double Current => _doubles[_index];

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext() => ++_index < _doubles.Count;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
