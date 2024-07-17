namespace Sudoku.Concepts;

public partial struct CandidateMap
{
	/// <summary>
	/// Indicates the internal buffer type.
	/// </summary>
	[InlineArray(12)]
	private struct BackingBuffer : IEquatable<BackingBuffer>, IEqualityOperators<BackingBuffer, BackingBuffer, bool>
	{
		/// <summary>
		/// Indicates the first element of the whole buffer.
		/// </summary>
		[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
		[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private long _firstElement;


		/// <inheritdoc cref="object.ToString"/>
		public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is BackingBuffer comparer && Equals(in comparer);

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public readonly bool Equals(ref readonly BackingBuffer other)
		{
			for (var i = 0; i < Length; i++)
			{
				if (this[i] != other[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			var hashCode = new HashCode();
			for (var i = 0; i < Length; i++)
			{
				hashCode.Add(this[i]);
			}
			return hashCode.ToHashCode();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly bool IEquatable<BackingBuffer>.Equals(BackingBuffer other) => Equals(in other);


		/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in BackingBuffer left, in BackingBuffer right) => left.Equals(in right);

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
