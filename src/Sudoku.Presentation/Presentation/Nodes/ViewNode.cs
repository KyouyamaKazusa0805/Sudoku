namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node.
/// </summary>
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
public abstract partial class ViewNode : ICloneable<ViewNode>, IEquatable<ViewNode>, IEqualityOperators<ViewNode, ViewNode, bool>
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
	[JsonInclude]
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

	/// <inheritdoc/>
	public abstract ViewNode Clone();
}
