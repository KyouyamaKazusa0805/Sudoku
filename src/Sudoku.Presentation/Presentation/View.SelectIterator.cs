#define ENHANCED_DRAWING_APIS
namespace Sudoku.Presentation;

partial class View
{
	/// <summary>
	/// Represents an enumerator that iterates for a list of elements that is projected by the current collection,
	/// converting by the specified converter.
	/// </summary>
	/// <typeparam name="T">The type of projected elements.</typeparam>
	public ref struct SelectIterator<T>
	{
		/// <summary>
		/// The selector method.
		/// </summary>
		private readonly Func<ViewNode, T> _selector;

		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private List<ViewNode>.Enumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="SelectIterator{T}"/> instance via the specified list of nodes.
		/// </summary>
		/// <param name="view">The internal nodes.</param>
		/// <param name="selector">The selector.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SelectIterator(View view, Func<ViewNode, T> selector) => (_enumerator, _selector) = (view._nodes.GetEnumerator(), selector);


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public T Current { get; private set; } = default!;


		/// <summary>
		/// Creates an <see cref="SelectIterator{T}"/> instance.
		/// </summary>
		/// <returns>An <see cref="SelectIterator{T}"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly SelectIterator<T> GetEnumerator() => this;

		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				Current = _selector(_enumerator.Current);
				return true;
			}

			return false;
		}
	}
}
