namespace Sudoku.Analytics.Construction.Chaining;

public partial record struct ForcingChainInfo
{
	/// <summary>
	/// Represents an enumerator type that can iterate on all nodes in the collection.
	/// </summary>
	/// <param name="_nodes">The collection of all nodes.</param>
	public ref struct Enumerator(HashSet<Node> _nodes) : IEnumerator<Node>
	{
		/// <summary>
		/// Indicates the backing enumerator.
		/// </summary>
		private HashSet<Node>.Enumerator _enumerator = _nodes.GetEnumerator();


		/// <inheritdoc/>
		public readonly Node Current => _enumerator.Current;

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc/>
		public readonly void Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		public void Reset() => throw new NotImplementedException();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
