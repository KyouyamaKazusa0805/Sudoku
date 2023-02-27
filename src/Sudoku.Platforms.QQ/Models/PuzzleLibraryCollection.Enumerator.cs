namespace Sudoku.Platforms.QQ.Models;

partial class PuzzleLibraryCollection
{
	/// <summary>
	/// Defines the internal enumerator of this type.
	/// </summary>
	public ref struct Enumerator
	{
		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private List<PuzzleLibraryData>.Enumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified <see cref="PuzzleLibraryCollection"/> instance.
		/// </summary>
		/// <param name="collection">The <see cref="PuzzleLibraryCollection"/> instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(PuzzleLibraryCollection collection) => _enumerator = collection.PuzzleLibraries.GetEnumerator();


		/// <inheritdoc cref="IEnumerator.Current"/>
		public PuzzleLibraryData Current { get; private set; } = null!;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				Current = _enumerator.Current;
				return true;
			}

			return false;
		}
	}
}
