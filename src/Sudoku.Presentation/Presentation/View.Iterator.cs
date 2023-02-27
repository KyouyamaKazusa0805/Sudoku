#define ENHANCED_DRAWING_APIS
namespace Sudoku.Presentation;

partial class View
{
	/// <summary>
	/// Represents with a default enumerator type that provides the mechanism of elementary operations
	/// used by <see langword="foreach"/> statements.
	/// </summary>
	public ref struct Iterator
	{
		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private List<ViewNode>.Enumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="Iterator"/> instance via the specified list of nodes.
		/// </summary>
		/// <param name="view">The internal nodes.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Iterator(View view) => _enumerator = view._nodes.GetEnumerator();


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public ViewNode Current { get; private set; } = null!;


		/// <summary>
		/// Determines whether the collection contains at least one element satisfying the specified condition.
		/// </summary>
		/// <param name="predicate">The condition.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public readonly bool Any(Predicate<ViewNode> predicate)
		{
			foreach (var element in this)
			{
				if (predicate(element))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Creates an <see cref="OfTypeIterator{T}"/> instance.
		/// </summary>
		/// <returns>An <see cref="OfTypeIterator{T}"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Iterator GetEnumerator() => this;

		/// <inheritdoc cref="IEnumerator.Current"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
