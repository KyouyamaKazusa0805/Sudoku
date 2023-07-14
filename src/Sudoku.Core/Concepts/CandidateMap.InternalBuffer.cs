namespace Sudoku.Concepts;

partial struct CandidateMap
{
	/// <summary>
	/// Indicates the internal buffer type.
	/// </summary>
	[InlineArray(12)]
	private struct InternalBuffer : IEquatable<InternalBuffer>
	{
		/// <summary>
		/// Indicates the first element of the whole buffer.
		/// </summary>
		[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
		[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private long _firstElement;


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is InternalBuffer comparer && Equals(comparer);

		/// <inheritdoc/>
		public readonly bool Equals(scoped in InternalBuffer other)
		{
			for (var i = 0; i < 12; i++)
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
			for (var i = 0; i < 12; i++)
			{
				hashCode.Add(this[i]);
			}

			return hashCode.ToHashCode();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly bool IEquatable<InternalBuffer>.Equals(InternalBuffer other) => Equals(other);


		/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(scoped in InternalBuffer left, scoped in InternalBuffer right) => left.Equals(right);

		/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(scoped in InternalBuffer left, scoped in InternalBuffer right) => !(left == right);
	}
}
