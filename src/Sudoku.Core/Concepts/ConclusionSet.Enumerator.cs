namespace Sudoku.Concepts;

public partial class ConclusionSet
{
	/// <summary>
	/// The internal enumerator instance.
	/// </summary>
	/// <param name="collection">The collection.</param>
	public ref struct Enumerator(ConclusionSet collection) : IEnumerator<Conclusion>
	{
		/// <summary>
		/// The conclusions to be iterated.
		/// </summary>
		private List<Conclusion>.Enumerator _enumerator = collection._conclusionsEntry.GetEnumerator();


		/// <summary>
		/// Indicates the current iterated element.
		/// </summary>
		public readonly Conclusion Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _enumerator.Current;
		}

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
