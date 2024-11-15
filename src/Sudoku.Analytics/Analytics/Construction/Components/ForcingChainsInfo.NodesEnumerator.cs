namespace Sudoku.Analytics.Construction.Components;

public partial record struct ForcingChainsInfo
{
	/// <summary>
	/// Represents an enumerator type that can iterate on nodes supposed with "on".
	/// </summary>
	/// <param name="_enumerator">The backing enumerator.</param>
	public ref struct NodesEnumerator(HashSet<Node>.Enumerator _enumerator) : IEnumerator<Node>
	{
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
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
