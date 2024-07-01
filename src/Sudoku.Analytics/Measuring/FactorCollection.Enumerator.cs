namespace Sudoku.Measuring;

public partial class FactorCollection
{
	/// <summary>
	/// The back enumerator of this type.
	/// </summary>
	/// <param name="factors">Indicates the back factors to be iterated.</param>
	public ref struct Enumerator(ReadOnlySpan<Factor> factors) : IEnumerator<Factor>
	{
		/// <summary>
		/// Indicates the field of factors.
		/// </summary>
		private readonly ReadOnlySpan<Factor> _factors = factors;

		/// <summary>
		/// Indicates the currently-iterated index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly Factor Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _factors[_index];
		}

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _factors.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
