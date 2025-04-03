namespace Sudoku.Concepts;

public partial struct CandidateMap
{
	/// <summary>
	/// Indicates the internal buffer type.
	/// </summary>
	[InlineArray(12)]
	private struct BackingBuffer :
		IEquatable<BackingBuffer>,
		IEqualityOperators<BackingBuffer, BackingBuffer, bool>,
		IInlineArray<BackingBuffer, ulong>
	{
		/// <summary>
		/// Indicates the first element of the whole buffer.
		/// </summary>
		private ulong _firstElement;


		/// <summary>
		/// Returns a sequence of <see cref="Vector256{T}"/> of <see cref="ulong"/> values that can be used in SIMD scenarios.
		/// </summary>
		public readonly ReadOnlySpan<Vector256<ulong>> Vectors
			=> (Vector256<ulong>[])[Vector256.Create(this[..4]), Vector256.Create(this[4..8]), Vector256.Create(this[8..])];

		/// <inheritdoc/>
		[UnscopedRef]
		readonly ReadOnlySpan<ulong> IInlineArray<BackingBuffer, ulong>.Elements => this[..];


		/// <inheritdoc/>
		static int IInlineArray<BackingBuffer, ulong>.InlineArrayLength => 12;


		/// <inheritdoc/>
		[UnscopedRef]
		ref ulong IInlineArray<BackingBuffer, ulong>.this[int index] => ref this[index];


		/// <inheritdoc cref="object.ToString"/>
		public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is BackingBuffer comparer && Equals(comparer);

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public readonly bool Equals(in BackingBuffer other)
		{
			var thisVectors = Vectors;
			var otherVectors = other.Vectors;
			return thisVectors[0] == otherVectors[0] && thisVectors[1] == otherVectors[1] && thisVectors[2] == otherVectors[2];
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			var hashCode = default(HashCode);
			for (var i = 0; i < Length; i++)
			{
				hashCode.Add(this[i]);
			}
			return hashCode.ToHashCode();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly bool IEquatable<BackingBuffer>.Equals(BackingBuffer other) => Equals(other);

		/// <inheritdoc/>
		[UnscopedRef]
		readonly ReadOnlySpan<ulong> IInlineArray<BackingBuffer, ulong>.AsReadOnlySpan() => this;

		/// <inheritdoc/>
		[UnscopedRef]
		ref ulong IInlineArray<BackingBuffer, ulong>.GetPinnableReference() => ref this[0];

		/// <inheritdoc/>
		[UnscopedRef]
		Span<ulong> IInlineArray<BackingBuffer, ulong>.AsSpan() => this;


		/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in BackingBuffer left, in BackingBuffer right) => left.Equals(right);

		/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in BackingBuffer left, in BackingBuffer right) => !(left == right);

		/// <inheritdoc/>
		static bool IEqualityOperators<BackingBuffer, BackingBuffer, bool>.operator ==(BackingBuffer left, BackingBuffer right)
			=> left == right;

		/// <inheritdoc/>
		static bool IEqualityOperators<BackingBuffer, BackingBuffer, bool>.operator !=(BackingBuffer left, BackingBuffer right)
			=> left != right;
	}
}
