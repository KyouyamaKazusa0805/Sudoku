namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	partial struct WhereEnumerator
	{
		/// <summary>
		/// Indicates the enumerator type that makes the projection from the <see cref="DrawingElement"/> to
		/// a specified target type <typeparamref name="T"/>, on the type <see cref="WhereEnumerator"/> iterating.
		/// </summary>
		/// <typeparam name="T">The type of the projection result.</typeparam>
		/// <seealso cref="DrawingElement"/>
		public unsafe ref partial struct SelectEnumerator<T>
		{
			/// <summary>
			/// The enunmerator.
			/// </summary>
			private WhereEnumerator _whereEnumerator;

			/// <summary>
			/// The selector.
			/// </summary>
			private readonly Func<DrawingElement, T>? _selector = null;

			/// <summary>
			/// The selector, but using function pointer.
			/// </summary>
			private readonly delegate*<DrawingElement, T> _selectorMethodPtr = null;


			/// <summary>
			/// Initializes an <see cref="SelectEnumerator{T}"/> instance via the details.
			/// </summary>
			/// <param name="enumerator">The enumerator.</param>
			/// <param name="selector">
			/// The method that produces a projection from <see cref="DrawingElement"/>
			/// to the target type <typeparamref name="T"/>.
			/// </param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal SelectEnumerator(WhereEnumerator enumerator, Func<DrawingElement, T> selector) :
				this(enumerator) => _selector = selector;

			/// <summary>
			/// Initializes an <see cref="SelectEnumerator{T}"/> instance via the details.
			/// </summary>
			/// <param name="enumerator">The enumerator.</param>
			/// <param name="selector">
			/// The function pointer that points to a method, which produces a projection
			/// from <see cref="DrawingElement"/> to the target type <typeparamref name="T"/>.
			/// </param>
			/// <exception cref="ArgumentNullException">
			/// Throws when the argument <paramref name="selector"/> is <see langword="null"/>.
			/// </exception>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal SelectEnumerator(WhereEnumerator enumerator, delegate*<DrawingElement, T> selector!!) :
				this(enumerator) => _selectorMethodPtr = selector;

			/// <summary>
			/// Initializes the items.
			/// </summary>
			/// <param name="enumerator">The enumerator.</param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private SelectEnumerator(WhereEnumerator enumerator) => _whereEnumerator = enumerator;


			/// <inheritdoc cref="IEnumerator{T}.Current"/>
			public readonly T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _whereEnumerator.Current is var e && _selectorMethodPtr == null ? _selector!(e) : _selectorMethodPtr(e);
			}


			/// <inheritdoc cref="IEnumerator.MoveNext"/>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext() => _whereEnumerator.MoveNext();

			/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly SelectEnumerator<T> GetEnumerator() => this;
		}
	}
}
