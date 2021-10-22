namespace Sudoku.Data;

/// <summary>
/// Defines a type of a link that constructed by 2 <see cref="ChainNode"/>s.
/// </summary>
/// <param name="TypeKind">
/// The <see cref="byte"/> value as the eigenvalue of the type. All possible values are:
/// <list type="table">
/// <item>
/// <term>0</term>
/// <description>The link is <see cref="ChainLinkTypes.Default"/>.</description>
/// </item>
/// <item>
/// <term>1</term>
/// <description>The link is <see cref="ChainLinkTypes.Weak"/>.</description>
/// </item>
/// <item>
/// <term>2</term>
/// <description>The link is <see cref="ChainLinkTypes.Strong"/>.</description>
/// </item>
/// <item>
/// <term>3</term>
/// <description>The link is <see cref="ChainLinkTypes.Line"/>.</description>
/// </item>
/// </list>
/// </param>
/// <completionlist cref="ChainLinkTypes"/>
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto, Pack = 0, Size = sizeof(byte))]
public readonly record struct ChainLinkType([field: FieldOffset(0)] byte TypeKind) : IValueEquatable<ChainLinkType>, IChainLinkType<ChainLinkType>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => TypeKind;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		TypeKind switch
		{
			0 => nameof(ChainLinkTypes.Default),
			1 => nameof(ChainLinkTypes.Weak),
			2 => nameof(ChainLinkTypes.Strong),
			3 => nameof(ChainLinkTypes.Line),
			_ => throw new InvalidOperationException("The value is out of range.")
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetNotation() => TypeKind switch { 0 => " -> ", 1 => " -- ", 2 => " == ", 3 => " -- " };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IValueEquatable<ChainLinkType>.Equals(in ChainLinkType other) => this == other;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ChainLinkType(byte value) => new(value);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator byte(ChainLinkType linkType) => linkType.TypeKind;
}
