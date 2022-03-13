namespace Sudoku.Test;

partial struct AlternatingInferenceChain
{
	/// <summary>
	/// Defines the inner enumerator that can enumerate all nodes of this chain.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// The inner data structure to store the nodes to be iterated.
		/// </summary>
		private readonly Node[] _nodes;

		/// <summary>
		/// Indicates the index being iterated.
		/// </summary>
		private int _index = -1;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the nodes to be iterated.
		/// </summary>
		/// <param name="nodes">The nodes to be iterated.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(Node[] nodes) => _nodes = nodes;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly Node Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _nodes[_index];
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _nodes.Length;
	}
}
