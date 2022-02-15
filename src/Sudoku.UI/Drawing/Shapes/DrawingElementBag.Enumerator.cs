namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	/// <summary>
	/// Indicates the enumerator type that allows iterating on <see cref="DrawingElementBag"/> instances. 
	/// </summary>
	/// <seealso cref="DrawingElementBag"/>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// The number of elements to be iterated.
		/// </summary>
		private readonly int _count;

		/// <summary>
		/// The list of elements.
		/// </summary>
		private readonly DrawingElement[] _elements;

		/// <summary>
		/// Indicates the index value that points to the current iterated value.
		/// </summary>
		private int _ptr = -1;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified elements and the iteration length.
		/// </summary>
		/// <param name="elements">The elements to be iterated.</param>
		/// <param name="count">The number of elements to be iterated.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(DrawingElement[] elements, int count)
		{
			_count = count;
			_elements = elements;
		}


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly DrawingElement Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _elements[_ptr];
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_ptr != _count;
	}
}
