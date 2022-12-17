#define ENHANCED_DRAWING_APIS

namespace Sudoku.Presentation;

partial class View
{
	partial struct WhereIterator
	{
		/// <summary>
		/// The filtering condition.
		/// </summary>
		private readonly Func<ViewNode, bool> _filteringCondition;

		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private List<ViewNode>.Enumerator _enumerator;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public ViewNode Current { get; private set; } = null!;


		/// <summary>
		/// Creates an <see cref="WhereIterator"/> instance.
		/// </summary>
		/// <returns>An <see cref="WhereIterator"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly WhereIterator GetEnumerator() => this;

		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			while (_enumerator.MoveNext())
			{
				if (_filteringCondition(_enumerator.Current))
				{
					Current = _enumerator.Current;
					return true;
				}
			}

			return false;
		}
	}
}
