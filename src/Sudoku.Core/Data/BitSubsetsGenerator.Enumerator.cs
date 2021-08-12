namespace Sudoku.Data
{
	partial struct BitSubsetsGenerator
	{
		/// <summary>
		/// Indicates the enumerator of the current instance.
		/// </summary>
		public ref partial struct Enumerator
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
			public Enumerator(int bitCount, int oneCount)
			{
				Current = (1 << oneCount) - 1;
				_mask = (1 << bitCount - oneCount) - 1;
				_isLast = bitCount == 0;
			}


			/// <inheritdoc/>
			public long Current { get; private set; }


			/// <inheritdoc/>
			public bool MoveNext()
			{
				bool result = hasNext(ref this);
				if (result && !_isLast)
				{
					long smallest = Current & -Current;
					long ripple = Current + smallest;
					long ones = Current ^ ripple;
					ones = (ones >> 2) / smallest;
					Current = ripple | ones;
				}

				return result;


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static bool hasNext(ref Enumerator @this)
				{
					bool result = !@this._isLast;
					@this._isLast = (@this.Current & -@this.Current & @this._mask) == 0;
					return result;
				}
			}
		}
	}
}
