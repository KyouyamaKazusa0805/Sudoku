using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
	/// <summary>
	/// Encapsulates a tree node.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	public sealed class TreeNode<T> : IComparable<TreeNode<T>?>, IEquatable<TreeNode<T>?>
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
		public bool IsLeaf => (Children?.Count ?? 0) == 0;

		/// <summary>
		/// Indicates the content.
		/// </summary>
		[MaybeNull]
		public T Content { get; set; } = default!;

		/// <summary>
		/// Indicates its children nodes.
		/// </summary>
		public ICollection<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();


		/// <inheritdoc/>
		public override bool Equals(object? obj) => CompareTo(obj as TreeNode<T>) == 0;

		/// <inheritdoc/>
		public bool Equals(TreeNode<T>? other) => CompareTo(other) == 0;

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			if (Content is not null)
			{
				return HashCode.Combine(Id, ParentId, IsLeaf, Content);
			}
			else
			{
				return HashCode.Combine(Id, ParentId, IsLeaf);
			}
		}

		/// <inheritdoc/>
		public int CompareTo(TreeNode<T>? other) => InternalCompare(this, other);

		/// <inheritdoc/>
		public override string ToString() => (Id, ParentId, Content, ChildrenCount: Children.Count).ToString();


		private static int InternalCompare(TreeNode<T>? left, TreeNode<T>? right) =>
			(left, right) switch
			{
				(null, null) => 0,
				(not null, not null) => left!.Id.CompareTo(right!.Id),
				_ => left is null ? -1 : 1
			};


		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(TreeNode<T>? left, TreeNode<T>? right) => InternalCompare(left, right) == 0;

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(TreeNode<T>? left, TreeNode<T>? right) => !(left == right);

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThan"]'/>
		public static bool operator >(TreeNode<T>? left, TreeNode<T>? right) => InternalCompare(left, right) > 0;

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThanOrEqual"]'/>
		public static bool operator >=(TreeNode<T>? left, TreeNode<T>? right) => InternalCompare(left, right) >= 0;

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_LessThan"]'/>
		public static bool operator <(TreeNode<T>? left, TreeNode<T>? right) => InternalCompare(left, right) < 0;

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_LessThanOrEqual"]'/>
		public static bool operator <=(TreeNode<T>? left, TreeNode<T>? right) => InternalCompare(left, right) <= 0;
	}
}
