namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	/// <summary>
	/// Indicates the enumerator type that allows iterating on <see cref="DrawingElementBag"/> instances,
	/// with 3 specified type parameters as the type of all iterated elements.
	/// If the type is either <typeparamref name="T1"/>, <typeparamref name="T2"/>
	/// or <typeparamref name="T3"/>, it'll be iterated.
	/// </summary>
	/// <typeparam name="T1">The type of the elements to be iterated.</typeparam>
	/// <typeparam name="T2">The type of the elements to be iterated.</typeparam>
	/// <typeparam name="T3">The type of the elements to be iterated.</typeparam>
	/// <seealso cref="DrawingElementBag"/>
	public ref partial struct OfTypeEnumerator<T1, T2, T3>
		where T1 : DrawingElement
		where T2 : DrawingElement
		where T3 : DrawingElement
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
		/// Initializes an <see cref="OfTypeEnumerator{T1, T2, T3}"/> instance via the details.
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
				if (Current is T1 or T2 or T3)
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OfTypeEnumerator<T1, T2, T3> GetEnumerator() => this;

		/// <summary>
		/// Gets the only element.
		/// </summary>
		/// <returns>The only element.</returns>
		/// <exception cref="InvalidOperationException">
		/// Throws when the enumerator can enumerate on multiple values, or the enumerator cannot step advanced.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly DrawingElement Single()
		{
			var enumerator = this;
			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("The list is empty.");
			}

			var result = enumerator.Current;
			return enumerator.MoveNext()
				? throw new InvalidOperationException("The list contains multiple elements.")
				: result;
		}

		/// <summary>
		/// Gets the only element, or <see langword="null"/> if the enumerator can iterates multiple values.
		/// </summary>
		/// <returns>The only element.</returns>
		/// <exception cref="InvalidOperationException">
		/// Throws when the enumerator cannot step advanced.
		/// </exception>
		public readonly DrawingElement? SingleOrDefault()
		{
			var enumerator = this;
			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("The list is empty.");
			}

			var result = enumerator.Current;
			return enumerator.MoveNext() ? null : result;
		}
	}
}
