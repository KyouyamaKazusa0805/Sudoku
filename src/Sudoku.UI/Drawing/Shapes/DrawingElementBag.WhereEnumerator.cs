namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	/// <summary>
	/// Indicates the enumerator type that makes the filtering on each element
	/// of type <see cref="DrawingElement"/>.
	/// </summary>
	/// <seealso cref="DrawingElement"/>
	public unsafe ref partial struct WhereEnumerator
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
		/// The predicate.
		/// </summary>
		private readonly Predicate<DrawingElement>? _predicate = null;

		/// <summary>
		/// The predicate, but using function pointer.
		/// </summary>
		private readonly delegate*<DrawingElement, bool> _predicateMethodPtr = null;

		/// <summary>
		/// Indicates the index value that points to the current iterated value.
		/// </summary>
		private int _ptr = -1;


		/// <summary>
		/// Initializes an <see cref="WhereEnumerator"/> instance via the details.
		/// </summary>
		/// <param name="elements">The elements to be iterated.</param>
		/// <param name="count">The number of elements to be iterated.</param>
		/// <param name="predicate">The filtering condition.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal WhereEnumerator(DrawingElement[] elements, int count, Predicate<DrawingElement> predicate) :
			this(elements, count) => _predicate = predicate;

		/// <summary>
		/// Initializes an <see cref="WhereEnumerator"/> instance via the details.
		/// </summary>
		/// <param name="elements">The elements to be iterated.</param>
		/// <param name="count">The number of elements to be iterated.</param>
		/// <param name="predicate">The filerting condition.</param>
		/// <exception cref="ArgumentNullException">
		/// Throws when the argument <paramref name="predicate"/> is <see langword="null"/>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal WhereEnumerator(DrawingElement[] elements, int count, delegate*<DrawingElement, bool> predicate!!) :
			this(elements, count) => _predicateMethodPtr = predicate;

		/// <summary>
		/// Initializes the items.
		/// </summary>
		/// <param name="elements">The elements.</param>
		/// <param name="count">The number of values to be iterated.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private WhereEnumerator(DrawingElement[] elements, int count)
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
		public bool MoveNext()
		{
			while (++_ptr != _count)
			{
				if (_predicateMethodPtr == null)
				{
					if (_predicate!(Current))
					{
						return true;
					}
				}
				else if (_predicateMethodPtr(Current))
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly WhereEnumerator GetEnumerator() => this;

		/// <summary>
		/// Makes a projection that converts each element to the target value of type <typeparamref name="T"/>,
		/// using the specified method to convert.
		/// </summary>
		/// <typeparam name="T">The type of the target result that each element converted.</typeparam>
		/// <param name="selector">The selector to convert the element.</param>
		/// <returns>
		/// The enumerator that allows you using <see langword="select"/> clause to get the result.
		/// </returns>
		/// <remarks>
		/// The method can be used by the following two ways:
		/// <list type="number">
		/// <item>
		/// Using query expression syntax:
		/// <c>
		/// var controls = from e in list where e is CellDigit select e.GetControl();
		/// </c>.
		/// </item>
		/// <item>
		/// Using method invocation syntax:
		/// <c>
		/// var controls = list.Where(static e => e is CellDigit).Select(static e => e.GetControl());
		/// </c>.
		/// </item>
		/// </list>
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly SelectEnumerator<T> Select<T>(Func<DrawingElement, T> selector) => new(this, selector);

		/// <summary>
		/// Makes a projection that converts each element to the target value of type <typeparamref name="T"/>,
		/// using the specified method to convert.
		/// </summary>
		/// <typeparam name="T">The type of the target result that each element converted.</typeparam>
		/// <param name="selector">The selector to convert the element.</param>
		/// <returns>The enumerator instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly SelectEnumerator<T> Select<T>(delegate*<DrawingElement, T> selector) => new(this, selector);
	}
}
