namespace Sudoku.Solving.Logical.Patterns;

partial class MultipleForcingChains
{
	partial struct Enumerator
	{
		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private SortedDictionary<byte, ChainNode>.Enumerator _enumerator;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public (byte CellOrDigit, ChainNode Potential) Current { get; private set; }


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				var (a, b) = _enumerator.Current;
				Current = (a, b);
				return true;
			}

			return false;
		}
	}
}
