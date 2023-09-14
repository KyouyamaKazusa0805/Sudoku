using System.Collections;
using System.Runtime.CompilerServices;

namespace Sudoku.Concepts;

partial class MultipleForcingChains
{
	/// <summary>
	/// Defines an enumerator that iterates the current collection.
	/// </summary>
	/// <param name="mfc">The <see cref="MultipleForcingChains"/> instance.</param>
	public ref struct Enumerator(MultipleForcingChains mfc)
	{
		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private SortedDictionary<byte, ChainNode>.Enumerator _enumerator = mfc._internalDictionary.GetEnumerator();


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
