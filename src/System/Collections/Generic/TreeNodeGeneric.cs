namespace System.Collections.Generic;

/// <summary>
/// Encapsulates a tree node.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
[AutoDeconstruct(nameof(Id), nameof(ParentId), nameof(IsLeaf), nameof(Content), nameof(Children))]
public sealed partial class TreeNode<T> : IComparable<TreeNode<T>?>, IEquatable<TreeNode<T>?>
{
	/// <summary>
	/// Indicates the current ID.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Indicates the parent ID of this instance.
	/// </summary>
	public int ParentId { get; set; }

	/// <summary>
	/// Indicates whether the current node is the left node.
	/// </summary>
	public bool IsLeaf => Children.Count == 0;

	/// <summary>
	/// Indicates the content.
	/// </summary>
	public T? Content { get; set; }

	/// <summary>
	/// Indicates its children nodes.
	/// </summary>
	public ICollection<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();


	/// <inheritdoc/>
	public override bool Equals(object? obj) => CompareTo(obj as TreeNode<T>) == 0;

	/// <inheritdoc/>
	public bool Equals(TreeNode<T>? other) => CompareTo(other) == 0;

	/// <inheritdoc/>
	public override int GetHashCode() =>
		Content is not null
		? HashCode.Combine(Id, ParentId, IsLeaf, Content)
		: HashCode.Combine(Id, ParentId, IsLeaf);

	/// <inheritdoc/>
	public int CompareTo(TreeNode<T>? other) => InternalCompare(this, other);

	/// <inheritdoc/>
	public override string ToString() => (Id, ParentId, Content, ChildrenCount: Children.Count).ToString();


	/// <summary>
	/// The internal comparison.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>The result.</returns>
	private static int InternalCompare(TreeNode<T>? left, TreeNode<T>? right) => (Left: left, Right: right) switch
	{
		(Left: null, Right: null) => 0,
		(Left: not null, Right: not null) => left.Id.CompareTo(right.Id),
		_ => left is null ? -1 : 1
	};


	/// <summary>
	/// Determines whether two <see cref="TreeNode{T}"/> instances hold a same inner value.
	/// </summary>
	/// <param name="left">The first instance to compare.</param>
	/// <param name="right">The second instance to compare.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public static bool operator ==(TreeNode<T>? left, TreeNode<T>? right) =>
		InternalCompare(left, right) == 0;

	/// <summary>
	/// Determines whether two <see cref="TreeNode{T}"/> instances don't hold a same inner value.
	/// </summary>
	/// <param name="left">The first instance to compare.</param>
	/// <param name="right">The second instance to compare.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public static bool operator !=(TreeNode<T>? left, TreeNode<T>? right) => !(left == right);

	/// <summary>
	/// Determines whether the left <see cref="TreeNode{T}"/> instance holds the greater <see cref="Id"/>
	/// value with the second <see cref="TreeNode{T}"/> instance.
	/// </summary>
	/// <param name="left">The first instance to compare.</param>
	/// <param name="right">The second instace to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <seealso cref="Id"/>
	public static bool operator >(TreeNode<T>? left, TreeNode<T>? right) =>
		InternalCompare(left, right) > 0;

	/// <summary>
	/// Determines whether the left <see cref="TreeNode{T}"/> instance holds the greater or same
	/// <see cref="Id"/> value with the second <see cref="TreeNode{T}"/> instance.
	/// </summary>
	/// <param name="left">The first instance to compare.</param>
	/// <param name="right">The second instace to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <seealso cref="Id"/>
	public static bool operator >=(TreeNode<T>? left, TreeNode<T>? right) =>
		InternalCompare(left, right) >= 0;

	/// <summary>
	/// Determines whether the left <see cref="TreeNode{T}"/> instance holds the less
	/// <see cref="Id"/> value with the second <see cref="TreeNode{T}"/> instance.
	/// </summary>
	/// <param name="left">The first instance to compare.</param>
	/// <param name="right">The second instace to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <seealso cref="Id"/>
	public static bool operator <(TreeNode<T>? left, TreeNode<T>? right) =>
		InternalCompare(left, right) < 0;

	/// <summary>
	/// Determines whether the left <see cref="TreeNode{T}"/> instance holds the less or same
	/// <see cref="Id"/> value with the second <see cref="TreeNode{T}"/> instance.
	/// </summary>
	/// <param name="left">The first instance to compare.</param>
	/// <param name="right">The second instace to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <seealso cref="Id"/>
	public static bool operator <=(TreeNode<T>? left, TreeNode<T>? right) =>
		InternalCompare(left, right) <= 0;
}
