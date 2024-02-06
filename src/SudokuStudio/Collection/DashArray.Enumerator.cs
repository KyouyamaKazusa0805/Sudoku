namespace SudokuStudio.Collection;

public partial struct DashArray
{
	/// <summary>
	/// Defines an enumerator of this type.
	/// </summary>
	/// <param name="doubles">The double values.</param>
	public ref struct Enumerator(List<double> doubles)
	{
		/// <summary>
		/// The internal array to be iterated.
		/// </summary>
		private readonly List<double> _doubles = doubles;

		/// <summary>
		/// Indicates the index of the current position.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly double Current => _doubles[_index];


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext() => ++_index < _doubles.Count;
	}
}
