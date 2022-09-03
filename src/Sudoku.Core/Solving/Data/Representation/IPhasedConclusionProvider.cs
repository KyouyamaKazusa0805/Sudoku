namespace Sudoku.Solving.Data.Representation;

/// <summary>
/// Defines a data structure that describes for a phased conclusion list.
/// </summary>
/// <typeparam name="TSelf">The type of the implemented type itself.</typeparam>
/// <typeparam name="TReasonEnum">
/// The type of the enumeration that represents the details and reasons why the conclusions can be used.
/// </typeparam>
public interface IPhasedConclusionProvider<TSelf, TReasonEnum> :
	IEquatable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>
	where TSelf : IPhasedConclusionProvider<TSelf, TReasonEnum>
	where TReasonEnum : unmanaged, Enum
{
	/// <summary>
	/// Indicates the conclusions that matches for the current reason.
	/// </summary>
	public Conclusion[] Conclusions { get; }

	/// <summary>
	/// Indicates the reason why the conclusions can be available.
	/// </summary>
	public TReasonEnum Reason { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	unsafe bool IEquatable<TSelf>.Equals([NotNullWhen(true)] TSelf? other)
	{
		return other switch
		{
			not null => Enumerable.SequenceEqual(Conclusions, other.Conclusions) && sizeof(TReasonEnum) switch
			{
				1 or 2 or 4 => asInt(Reason) == asInt(other.Reason),
				8 => asLong(Reason) == asLong(other.Reason),
				_ => false
			},
			_ => false
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int asInt(TReasonEnum e) => Unsafe.As<TReasonEnum, int>(ref e);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static long asLong(TReasonEnum e) => Unsafe.As<TReasonEnum, long>(ref e);
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	public sealed int GetHashCode()
	{
		var hashCode = new HashCode();
		foreach (var conclusion in Conclusions)
		{
			hashCode.Add(conclusion);
		}

		hashCode.Add(Reason);
		return hashCode.ToHashCode();
	}

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed OneDimensionalArrayEnumerator<Conclusion> GetEnumerator() => Conclusions.EnumerateImmutable();
}
