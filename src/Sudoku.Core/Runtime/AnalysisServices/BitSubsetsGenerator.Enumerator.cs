namespace Sudoku.Runtime.AnalysisServices;

partial struct BitSubsetsGenerator
{
	/// <summary>
	/// Indicates the enumerator of the current instance.
	/// </summary>
	public ref struct Enumerator
	{
		/// <summary>
		/// The mask.
		/// </summary>
		private readonly long _mask;


		/// <summary>
		/// Indicates whether that the value is the last one.
		/// </summary>
		private bool _isLast;


		/// <summary>
		/// Initializes an instance with the specified number of bits
		/// and <see langword="true"/> bits.
		/// </summary>
		/// <param name="bitCount">The number of bits.</param>
		/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(int bitCount, int oneCount)
			=> (Current, _mask, _isLast) = ((1 << oneCount) - 1, (1 << bitCount - oneCount) - 1, bitCount == 0);


		/// <inheritdoc/>
		public long Current { get; private set; }


		/// <inheritdoc/>
		public bool MoveNext()
		{
			var result = hasNext(ref this);
			if (result && !_isLast)
			{
				var smallest = Current & -Current;
				var ripple = Current + smallest;
				var ones = Current ^ ripple;
				ones = (ones >> 2) / smallest;
				Current = ripple | ones;
			}

			return result;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool hasNext(ref Enumerator @this)
			{
				var result = !@this._isLast;
				@this._isLast = (@this.Current & -@this.Current & @this._mask) == 0;
				return result;
			}
		}
	}
}
