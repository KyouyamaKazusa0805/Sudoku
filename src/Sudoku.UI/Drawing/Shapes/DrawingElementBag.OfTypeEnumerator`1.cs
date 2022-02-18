namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	/// <summary>
	/// Indicates the enumerator type that allows iterating on <see cref="DrawingElementBag"/> instances,
	/// with the specified type parameter as the type of all iterated elements.
	/// </summary>
	/// <typeparam name="TDrawingElement">The type of the elements to be iterated.</typeparam>
	/// <seealso cref="DrawingElementBag"/>
	public ref partial struct OfTypeEnumerator<TDrawingElement> where TDrawingElement : DrawingElement
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
		/// Initializes an <see cref="OfTypeEnumerator{TDrawingElement}"/> instance via the details.
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
		public readonly TDrawingElement Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (TDrawingElement)_elements[_ptr];
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			while (++_ptr != _count)
			{
				if (_elements[_ptr] is TDrawingElement)
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OfTypeEnumerator<TDrawingElement> GetEnumerator() => this;

		/// <summary>
		/// Gets the only element.
		/// </summary>
		/// <returns>The only element.</returns>
		/// <exception cref="InvalidOperationException">
		/// Throws when the enumerator can enumerate on multiple values, or the enumerator cannot step advanced.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly TDrawingElement Single()
		{
			var enumerator = this;
			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("The list is empty.");
			}

			var result = enumerator.Current;
			if (!enumerator.MoveNext())
			{
				return result;
			}

			throw new InvalidOperationException("The list contains multiple elements.");
		}

		/// <summary>
		/// Gets the only element, or <see langword="null"/> if the enumerator can iterates multiple values.
		/// </summary>
		/// <returns>The only element.</returns>
		/// <exception cref="InvalidOperationException">
		/// Throws when the enumerator cannot step advanced.
		/// </exception>
		public readonly TDrawingElement? SingleOrDefault()
		{
			var enumerator = this;
			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("The list is empty.");
			}

			var result = enumerator.Current;
			if (!enumerator.MoveNext())
			{
				return result;
			}

			return null;
		}
	}
}
