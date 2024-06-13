namespace Sudoku.Concepts;

public partial class ChainOrLoop
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each <see cref="Node"/> inside the collection.
	/// </summary>
	/// <param name="nodes">The <see cref="ChainOrLoop"/> instance.</param>
	public ref struct Enumerator(ChainOrLoop nodes)
	{
		/// <summary>
		/// Indicates the number of nodes.
		/// </summary>
		private readonly int _length = nodes.Length;

		/// <summary>
		/// Indicates the current index iterated.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly Node Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => nodes[_index];
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _length;
	}
}
