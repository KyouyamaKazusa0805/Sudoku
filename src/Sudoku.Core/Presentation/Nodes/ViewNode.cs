namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node.
/// </summary>
public abstract class ViewNode : ICloneable, IEquatable<ViewNode>, IEqualityOperators<ViewNode, ViewNode>
{
	/// <summary>
	/// Assigns the <see cref="Presentation.Identifier"/> instance as the basic information.
	/// </summary>
	/// <param name="identifier">The <see cref="Presentation.Identifier"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected ViewNode(Identifier identifier) => Identifier = identifier;


	/// <summary>
	/// Indicates the identifier used.
	/// </summary>
	public Identifier Identifier { get; protected set; }

	/// <summary>
	/// Indicates the inner identifier to distinct the different types that is derived from <see cref="ViewNode"/>.
	/// </summary>
	/// <seealso cref="ViewNode"/>
	protected abstract string TypeIdentifier { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is ViewNode comparer && Equals(comparer);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] ViewNode? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <inheritdoc/>
	public abstract override string ToString();

	/// <summary>
	/// Creates a new instance whose inner data is totally same as the current instance.
	/// </summary>
	/// <returns>The result <see cref="ViewNode"/> as the copy.</returns>
	public abstract ViewNode Clone();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	object ICloneable.Clone() => Clone();


	/// <summary>
	/// Determines whether two <see cref="ViewNode"/>s are same type, and hold a same value.
	/// </summary>
	/// <param name="left">The first instance to be compared.</param>
	/// <param name="right">The second instance to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ViewNode? left, ViewNode? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <summary>
	/// Determines whether two <see cref="ViewNode"/>s are not same type, or not totally hold a same value.
	/// </summary>
	/// <param name="left">The first instance to be compared.</param>
	/// <param name="right">The second instance to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ViewNode? left, ViewNode? right) => !(left == right);
}
