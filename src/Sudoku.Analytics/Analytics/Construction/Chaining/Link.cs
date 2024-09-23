namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// <para>Represents a link that describes a relation between two <see cref="Node"/> instances.</para>
/// <para><b>
/// Please note that two <see cref="Link"/> instances will be considered as equal
/// only if they holds same node values, regardless of what link type two <see cref="Link"/> instances use.
/// </b></para>
/// </summary>
/// <param name="firstNode">Indicates the first node to be used.</param>
/// <param name="secondNode">Indicates the second node to be used.</param>
/// <param name="isStrong">Indicates whether the link type is a strong link or not.</param>
/// <param name="groupedLinkPattern">
/// Indicates the pattern that the grouped link used. The value can be used as a "tag" recording extra information.
/// The default value is <see langword="null"/>.
/// </param>
/// <seealso cref="Node"/>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.EqualityOperators)]
public sealed partial class Link(
	[PrimaryConstructorParameter] Node firstNode,
	[PrimaryConstructorParameter] Node secondNode,
	[PrimaryConstructorParameter] bool isStrong,
	[PrimaryConstructorParameter] object? groupedLinkPattern = null
) : IEquatable<Link>, IEqualityOperators<Link, Link, bool>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Link? other) => other is not null && Equals(other, LinkComparison.Undirected);

	/// <summary>
	/// Determine whether two <see cref="Link"/> are considered equal on the specified comparison rule.
	/// </summary>
	/// <param name="other">The other object to be compared.</param>
	/// <param name="comparison">The comparison rule to be used.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="comparison"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Link other, LinkComparison comparison)
		=> Enum.IsDefined(comparison)
			? IsStrong == other.IsStrong && comparison switch
			{
				LinkComparison.Undirected
					=> FirstNode == other.FirstNode && SecondNode == other.SecondNode
					|| FirstNode == other.SecondNode && SecondNode == other.FirstNode,
				_ => FirstNode == other.FirstNode && SecondNode == other.SecondNode
			}
			: throw new ArgumentOutOfRangeException(nameof(comparison));

	/// <inheritdoc/>
	public override int GetHashCode() => GetHashCode(LinkComparison.Undirected);

	/// <summary>
	/// Serves as hash code functions, with consideration on the specified comparison rule.
	/// </summary>
	/// <param name="comparison">The comparison rule.</param>
	/// <returns>An <see cref="int"/> indicating the hash code.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="comparison"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(LinkComparison comparison)
		=> Enum.IsDefined(comparison)
			? comparison switch
			{
				LinkComparison.Undirected => HashCode.Combine(FirstNode.GetHashCode() ^ SecondNode.GetHashCode()),
				_ => HashCode.Combine(FirstNode, SecondNode)
			}
			: throw new ArgumentOutOfRangeException(nameof(comparison));

	/// <inheritdoc/>
	public override string ToString()
		=> $"{FirstNode}{(IsStrong ? Inference.Strong : Inference.Weak).ConnectingNotation()}{SecondNode}";
}
