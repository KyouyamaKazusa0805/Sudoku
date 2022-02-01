namespace Sudoku.Collections;

partial struct NodeSet
{
	/// <summary>
	/// Indicates the enumerator.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Indicates the length of elements to iterate.
		/// </summary>
		private readonly int _length;

		/// <summary>
		/// Indicates the list of nodes.
		/// </summary>
		private readonly Node[] _chainNodes;


		/// <summary>
		/// Indicates the index that the current enumerator points to.
		/// </summary>
		private int _index = -1;


		/// <summary>
		/// Initializes a set of <see cref="Node"/>s.
		/// </summary>
		/// <param name="chainNodes">The chain nodes.</param>
		/// <param name="length">Indicates the number of nodes to iterate.</param>
		internal Enumerator(Node[] chainNodes, int length)
		{
			_chainNodes = chainNodes;
			_length = length;
		}


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly Node Current => _chainNodes[_index];


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext() => ++_index <= _length;
	}
}
