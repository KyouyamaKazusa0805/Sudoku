namespace Sudoku.Analytics;

partial class ConclusionSet
{
	/// <summary>
	/// The internal enumerator instance.
	/// </summary>
	/// <param name="collection">The collection.</param>
	public ref struct Enumerator(ConclusionSet collection)
	{
		/// <summary>
		/// The conclusions to be iterated.
		/// </summary>
		private List<Conclusion>.Enumerator _enumerator = collection._conclusionsEntry.GetEnumerator();


		/// <summary>
		/// Indicates the current iterated element.
		/// </summary>
		public Conclusion Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _enumerator.Current;
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
