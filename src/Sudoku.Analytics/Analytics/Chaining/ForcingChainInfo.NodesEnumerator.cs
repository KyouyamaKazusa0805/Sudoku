namespace Sudoku.Analytics.Chaining;

public partial record struct ForcingChainInfo
{
	/// <summary>
	/// Represents an enumerator type that can iterate on nodes supposed with "on".
	/// </summary>
	/// <param name="enumerator">The backing enumerator.</param>
	public ref struct NodesEnumerator(HashSet<Node>.Enumerator enumerator) : IEnumerator<Node>
	{
		/// <inheritdoc/>
		public readonly Node Current => enumerator.Current;

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc/>
		public readonly void Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		public void Reset() => throw new NotImplementedException();

		/// <inheritdoc/>
		public bool MoveNext() => enumerator.MoveNext();
	}
}
