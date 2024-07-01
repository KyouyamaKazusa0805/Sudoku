namespace Sudoku.Concepts;

public partial struct EliminationRange
{
	/// <summary>
	/// Indicates the enumerator of the current instance.
	/// </summary>
	/// <param name="range">The range to be used.</param>
	public ref struct Enumerator(ref readonly EliminationRange range) : IEnumerator<KeyValuePair<Digit, HouseMask>>
	{
		/// <summary>
		/// Indicates the internal field.
		/// </summary>
		private readonly ref readonly EliminationRange _range = ref range;

		/// <summary>
		/// Indicates the current digit index iterated.
		/// </summary>
		private int _digitIndex = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly KeyValuePair<Digit, HouseMask> Current => new(_digitIndex, _range.GetAtRef(_digitIndex));

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			while (++_digitIndex < 9)
			{
				if (_range[_digitIndex] != 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
