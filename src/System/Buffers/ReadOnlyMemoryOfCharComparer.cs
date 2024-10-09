namespace System.Buffers;

/// <summary>
/// Represents an equality comparer of <see cref="ReadOnlySpan{T}"/> of <see cref="char"/> object,
/// but it can also be created by comparing <see cref="string"/> values as an efficient way in variant collection types
/// like <see cref="HashSet{T}.AlternateLookup{TAlternate}"/>.
/// </summary>
/// <seealso cref="HashSet{T}.AlternateLookup{TAlternate}"/>
public sealed class ReadOnlyMemoryOfCharComparer :
	IEqualityComparer<ReadOnlyCharMemorySequence>,
	IAlternateEqualityComparer<ReadOnlyCharSequence, ReadOnlyCharMemorySequence>
{
	/// <summary>
	/// Indicates the singleton object of this type.
	/// </summary>
	public static readonly ReadOnlyMemoryOfCharComparer Instance = new();


	/// <summary>
	/// Initializes a <see cref="ReadOnlyMemoryOfCharComparer"/> instance.
	/// </summary>
	private ReadOnlyMemoryOfCharComparer()
	{
	}


	/// <inheritdoc/>
	public bool Equals(ReadOnlyCharMemorySequence x, ReadOnlyCharMemorySequence y) => x.Span.SequenceEqual(y.Span);

	/// <inheritdoc/>
	public bool Equals(ReadOnlyCharSequence alternate, ReadOnlyCharMemorySequence other) => alternate.SequenceEqual(other.Span);

	/// <inheritdoc/>
	public int GetHashCode(ReadOnlyCharMemorySequence obj) => GetHashCode(obj.Span);

	/// <inheritdoc/>
	public int GetHashCode(ReadOnlyCharSequence alternate)
	{
		var hc = default(HashCode);
		hc.AddBytes(MemoryMarshal.Cast<char, byte>(alternate));
		return hc.ToHashCode();
	}

	/// <inheritdoc/>
	public ReadOnlyCharMemorySequence Create(ReadOnlyCharSequence alternate) => alternate.ToArray();
}
