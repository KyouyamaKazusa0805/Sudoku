namespace Sudoku.Resources;

public partial struct InterpolationArray
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each element of an <see cref="InterpolationArray"/> instance.
	/// </summary>
	/// <param name="values">The internal values.</param>
	/// <seealso cref="InterpolationArray"/>
	public ref struct Enumerator(ref readonly InterpolationArray values) : IEnumerator<Interpolation>
	{
		/// <summary>
		/// Indicates the backing span.
		/// </summary>
		private readonly ReadOnlySpan<Interpolation> _values = values.Span;

		/// <summary>
		/// Indicates the current index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly Interpolation Current => _values[_index];

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _values.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
