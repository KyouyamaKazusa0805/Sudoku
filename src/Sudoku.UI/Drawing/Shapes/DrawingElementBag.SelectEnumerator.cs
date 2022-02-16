namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	/// <summary>
	/// Indicates the enumerator type that makes the projection from the <see cref="DrawingElement"/> to
	/// a specified target type <typeparamref name="T"/>. 
	/// </summary>
	/// <typeparam name="T">The type of the projection result.</typeparam>
	/// <seealso cref="DrawingElement"/>
	public unsafe ref partial struct SelectEnumerator<T>
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
		/// The selector.
		/// </summary>
		private readonly Func<DrawingElement, T>? _selector = null;

		/// <summary>
		/// The selector, but using function pointer.
		/// </summary>
		private readonly delegate*<DrawingElement, T> _selectorMethodPtr = null;

		/// <summary>
		/// Indicates the index value that points to the current iterated value.
		/// </summary>
		private int _ptr = -1;


		/// <summary>
		/// Initializes an <see cref="SelectEnumerator{T}"/> instance via the details.
		/// </summary>
		/// <param name="elements">The elements to be iterated.</param>
		/// <param name="count">The number of elements to be iterated.</param>
		/// <param name="selector">
		/// The method that produces a projection from <see cref="DrawingElement"/>
		/// to the target type <typeparamref name="T"/>.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SelectEnumerator(DrawingElement[] elements, int count, Func<DrawingElement, T> selector) :
			this(elements, count) => _selector = selector;

		/// <summary>
		/// Initializes an <see cref="SelectEnumerator{T}"/> instance via the details.
		/// </summary>
		/// <param name="elements">The elements to be iterated.</param>
		/// <param name="count">The number of elements to be iterated.</param>
		/// <param name="selector">
		/// The function pointer that points to a method, which produces a projection
		/// from <see cref="DrawingElement"/> to the target type <typeparamref name="T"/>.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Throws when the argument <paramref name="selector"/> is <see langword="null"/>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SelectEnumerator(DrawingElement[] elements, int count, delegate*<DrawingElement, T> selector!!) :
			this(elements, count) => _selectorMethodPtr = selector;

		/// <summary>
		/// Initializes the items.
		/// </summary>
		/// <param name="elements">The elements.</param>
		/// <param name="count">The number of values to be iterated.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private SelectEnumerator(DrawingElement[] elements, int count)
		{
			_count = count;
			_elements = elements;
		}


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly T Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _elements[_ptr] is var e && _selectorMethodPtr == null ? _selector!(e) : _selectorMethodPtr(e);
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_ptr != _count;

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly SelectEnumerator<T> GetEnumerator() => this;
	}
}
