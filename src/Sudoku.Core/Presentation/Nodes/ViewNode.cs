namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node.
/// </summary>
[AutoOverloadsEqualityOperators(WithNullableAnnotation = true)]
public abstract partial class ViewNode : ICloneable, IEquatable<ViewNode>, IEqualityOperators<ViewNode, ViewNode>
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
}
