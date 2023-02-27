namespace Sudoku.Solving.Logical.Patterns;

partial class MultipleForcingChains
{
	/// <summary>
	/// Defines an enumerator that iterates the current collection.
	/// </summary>
	public ref struct Enumerator
	{
		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private SortedDictionary<byte, ChainNode>.Enumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified <see cref="MultipleForcingChains"/> instance.
		/// </summary>
		/// <param name="mfc">The <see cref="MultipleForcingChains"/> instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(MultipleForcingChains mfc) => _enumerator = mfc._internalDictionary.GetEnumerator();


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
