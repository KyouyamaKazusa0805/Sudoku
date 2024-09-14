namespace Sudoku.Analytics.Patterning.Chaining;

public partial class ChainOrLoop
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each <see cref="Node"/> inside the collection.
	/// </summary>
	/// <param name="_nodes">The <see cref="ChainOrLoop"/> instance.</param>
	public ref struct Enumerator(ChainOrLoop _nodes) : IEnumerator<Node>
	{
		/// <summary>
		/// Indicates the current index iterated.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly Node Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _nodes[_index];
		}

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _nodes.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
