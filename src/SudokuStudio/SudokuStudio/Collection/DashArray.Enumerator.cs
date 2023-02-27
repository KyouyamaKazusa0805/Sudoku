namespace SudokuStudio.Collection;

partial struct DashArray
{
	/// <summary>
	/// Defines an enumerator of this type.
	/// </summary>
	public ref struct Enumerator
	{
		/// <summary>
		/// The internal array to be iterated.
		/// </summary>
		private readonly double[] _doubles;

		/// <summary>
		/// Indicates the index of the current position.
		/// </summary>
		private int _index = -1;


		/// <summary>
		/// Initializes a <see cref="Enumerator"/> instance.
		/// </summary>
		/// <param name="doubles">The double values.</param>
		internal Enumerator(double[] doubles) => _doubles = doubles;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly double Current => _doubles[_index];


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext() => ++_index < _doubles.Length;
	}
}
