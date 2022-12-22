namespace Sudoku.Platforms.QQ.Models;

partial class PuzzleLibraryCollection
{
	partial struct Enumerator
	{
		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private List<PuzzleLibraryData>.Enumerator _enumerator;


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
