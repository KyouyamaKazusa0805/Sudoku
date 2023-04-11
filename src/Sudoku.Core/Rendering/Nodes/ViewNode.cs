namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node.
/// </summary>
/// <param name="identifier">The <see cref="Rendering.Identifier"/> instance.</param>
public abstract partial class ViewNode(Identifier identifier) : ICloneable<ViewNode>, IEquatable<ViewNode>, IEqualityOperators<ViewNode, ViewNode, bool>
{
	/// <summary>
	/// Indicates the identifier used.
	/// </summary>
	[JsonInclude]
	public Identifier Identifier { get; protected set; } = identifier;

	/// <summary>
	/// Indicates the inner identifier to distinct the different types that is derived from <see cref="ViewNode"/>.
	/// </summary>
	/// <seealso cref="ViewNode"/>
	protected string TypeIdentifier => GetType().Name;


	[GeneratedOverridingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public sealed override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] ViewNode? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <inheritdoc/>
	public abstract override string ToString();

	/// <inheritdoc/>
	public abstract ViewNode Clone();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ViewNode? left, ViewNode? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ViewNode? left, ViewNode? right) => !(left == right);
}
