namespace Sudoku.Data;

/// <summary>
/// Defines a type of a link that constructed by 2 <see cref="ChainNode"/>s.
/// </summary>
/// <param name="TypeKind">The <see cref="byte"/> value as the eigenvalue of the type.</param>
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto, Pack = 0, Size = sizeof(byte))]
[DiscriminatedUnion(ExceptionThrowsWhenOutOfRange = true)]
public readonly record struct ChainLinkType([field: FieldOffset(0)] byte TypeKind) : IChainLinkType<ChainLinkType>
{
	/// <summary>
	/// Indicates the default link.
	/// </summary>
	public static readonly ChainLinkType Default = new(0);

	/// <summary>
	/// Indicates the weak link.
	/// </summary>
	public static readonly ChainLinkType Weak = new(1);

	/// <summary>
	/// Indicates the strong link.
	/// </summary>
	public static readonly ChainLinkType Strong = new(2);

	/// <summary>
	/// Indicates the line link.
	/// </summary>
	public static readonly ChainLinkType Line = new(3);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => TypeKind;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		TypeKind switch
		{
			0 => nameof(Default),
			1 => nameof(Weak),
			2 => nameof(Strong),
			3 => nameof(Line),
			_ => throw new InvalidOperationException("The value is out of range.")
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetNotation() => TypeKind switch { 0 => " -> ", 1 => " -- ", 2 => " == ", 3 => " -- " };


	/// <summary>
	/// Explicit cast from <see cref="byte"/> to <see cref="ChainLinkType"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ChainLinkType(byte value) => new(value);

	/// <summary>
	/// Explicit cast from <see cref="ChainLinkType"/> to <see cref="byte"/>.
	/// </summary>
	/// <param name="linkType">The link type.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator byte(ChainLinkType linkType) => linkType.TypeKind;
}
