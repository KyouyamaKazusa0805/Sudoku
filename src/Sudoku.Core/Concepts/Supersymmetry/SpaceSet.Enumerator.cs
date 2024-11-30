namespace Sudoku.Concepts.Supersymmetry;

public partial struct SpaceSet
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each <see cref="Space"/> of the current type.
	/// </summary>
	/// <param name="set">The set instance.</param>
	public ref struct Enumerator(ref readonly SpaceSet set) : IEnumerator<Space>
	{
		/// <summary>
		/// Indicates the set.
		/// </summary>
		private readonly ReadOnlySpan<Space> _set = set.ToArray();

		/// <summary>
		/// Indicates the current index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc/>
		public readonly Space Current => _set[_index];

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc/>
		public bool MoveNext() => ++_index < _set.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
