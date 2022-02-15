namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	/// <summary>
	/// Indicates the enumerator type that allows iterating on <see cref="DrawingElementBag"/> instances,
	/// with 2 specified type parameters as the type of all iterated elements.
	/// If the type is either <typeparamref name="T1"/> or <typeparamref name="T2"/>, it'll be iterated.
	/// </summary>
	/// <typeparam name="T1">The first type of the elements to be iterated.</typeparam>
	/// <typeparam name="T2">The second type of the elements to be iterated.</typeparam>
	/// <seealso cref="DrawingElementBag"/>
	public ref partial struct OfTypeEnumerator<T1, T2> where T1 : DrawingElement where T2 : DrawingElement
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
		internal OfTypeEnumerator(DrawingElement[] elements, int count)
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
		public bool MoveNext()
		{
			while (++_ptr != _count)
			{
				if (Current is T1 or T2)
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OfTypeEnumerator<T1, T2> GetEnumerator() => this;
	}
}
