#define ENHANCED_DRAWING_APIS

namespace Sudoku.Presentation;

partial class View
{
	partial struct SelectIterator<T>
	{
		/// <summary>
		/// The selector method.
		/// </summary>
		private readonly Func<ViewNode, T> _selector;

		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private List<ViewNode>.Enumerator _enumerator;


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
