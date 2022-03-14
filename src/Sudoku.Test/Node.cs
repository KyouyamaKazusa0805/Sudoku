namespace Sudoku.Test;

/// <summary>
/// Defines a chain node.
/// </summary>
public abstract class Node :
	ICloneable,
	IEquatable<Node>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Node, Node>
#endif
{
	/// <summary>
	/// Indicates the type of the node.
	/// </summary>
	public abstract NodeType Type { get; }


	/// <inheritdoc cref="ICloneable.Clone"/>
	public abstract Node Clone();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as Node);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] Node? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <inheritdoc/>
	public abstract override string ToString();

	/// <summary>
	/// Gets the simplified string value that only displays the important information.
	/// </summary>
	/// <returns>The string value.</returns>
	public abstract string ToSimpleString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	object ICloneable.Clone() => Clone();


	/// <summary>
	/// Determines whether two <see cref="Node"/>s are same.
	/// </summary>
	/// <param name="left">Indicates the left-side instance to compare.</param>
	/// <param name="right">Indicates the right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Node? left, Node? right) =>
		(left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <summary>
	/// Determines whether two <see cref="Node"/>s are not totally same.
	/// </summary>
	/// <param name="left">Indicates the left-side instance to compare.</param>
	/// <param name="right">Indicates the right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Node? left, Node? right) => !(left == right);
}
