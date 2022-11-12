namespace Sudoku.Presentation;

partial struct ValueView
{
	partial struct SegmentEnumerator
	{
		/// <summary>
		/// The inner enumerator.
		/// </summary>
		private LinkedList<ViewNodeSegment>.Enumerator _enumerator;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public ViewNodeSegment Current => _enumerator.Current;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();


		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly SegmentEnumerator GetEnumerator() => this;
	}
}
