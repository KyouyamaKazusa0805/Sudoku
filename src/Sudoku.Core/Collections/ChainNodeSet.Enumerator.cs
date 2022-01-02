namespace Sudoku.Collections;

partial struct ChainNodeSet
{
	/// <summary>
	/// Indicates the enumerator.
	/// </summary>
	public ref struct Enumerator
	{
		/// <summary>
		/// Indicates the length of elements to iterate.
		/// </summary>
		private readonly int _length;

		/// <summary>
		/// Indicates the list of nodes.
		/// </summary>
		private readonly ChainNode[] _chainNodes;


		/// <summary>
		/// Indicates the index that the current enumerator points to.
		/// </summary>
		private int _index = -1;


		/// <summary>
		/// Initializes a set of <see cref="ChainNode"/>s.
		/// </summary>
		/// <param name="chainNodes">The chain nodes.</param>
		/// <param name="length">Indicates the number of nodes to iterate.</param>
		internal Enumerator(ChainNode[] chainNodes, int length)
		{
			_chainNodes = chainNodes;
			_length = length;
		}


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly ChainNode Current => _chainNodes[_index];


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext() => ++_index <= _length;
	}
}
