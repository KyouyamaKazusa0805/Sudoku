namespace Sudoku.Data;

/// <summary>
/// Defines a type of a link that constructed by 2 <see cref="Node"/>s.
/// </summary>
/// <param name="TypeKind">
/// The <see cref="byte"/> value as the eigenvalue of the type. All possible values are:
/// <list type="table">
/// <item>
/// <term>0</term>
/// <description>The link is <see cref="LinkTypes.Default"/>.</description>
/// </item>
/// <item>
/// <term>1</term>
/// <description>The link is <see cref="LinkTypes.Weak"/>.</description>
/// </item>
/// <item>
/// <term>2</term>
/// <description>The link is <see cref="LinkTypes.Strong"/>.</description>
/// </item>
/// <item>
/// <term>3</term>
/// <description>The link is <see cref="LinkTypes.Line"/>.</description>
/// </item>
/// </list>
/// </param>
public readonly record struct LinkType(byte TypeKind) : ILinkType<LinkType>
{
	/// <summary>
	/// Determine whether the specified <see cref="LinkType"/> instance
	/// holds the same type kind as the current one.
	/// </summary>
	/// <param name="other">The other instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(LinkType other) => TypeKind == other.TypeKind;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => TypeKind;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		TypeKind switch
		{
			0 => nameof(LinkTypes.Default),
			1 => nameof(LinkTypes.Weak),
			2 => nameof(LinkTypes.Strong),
			3 => nameof(LinkTypes.Line),
			_ => throw new InvalidOperationException("The value is out of range.")
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetNotation() => TypeKind switch { 0 => " -> ", 1 => " -- ", 2 => " == ", 3 => " -- " };


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator LinkType(byte value) => new(value);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator byte(LinkType linkType) => linkType.TypeKind;
}
