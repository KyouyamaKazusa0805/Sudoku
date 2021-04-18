using Sudoku.DocComments;

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
		public bool IsLeaf => Children.Count == 0;

		/// <summary>
		/// Indicates the content.
		/// </summary>
		public T? Content { get; set; }

		/// <summary>
		/// Indicates its children nodes.
		/// </summary>
		public ICollection<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="id">The ID.</param>
		/// <param name="parentId">The parent ID.</param>
		/// <param name="isLeaf">Indicates whether the node is leaf.</param>
		/// <param name="content">The content.</param>
		/// <param name="children">All children.</param>
		public void Deconstruct(
			out int id, out int parentId, out bool isLeaf, out T? content, out ICollection<TreeNode<T>> children)
		{
			id = Id;
			parentId = ParentId;
			isLeaf = IsLeaf;
			content = Content;
			children = Children;
		}


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
		private static int InternalCompare(TreeNode<T>? left, TreeNode<T>? right) => (left, right) switch
		{
			(null, null) => 0,
			(not null, not null) => left!.Id.CompareTo(right!.Id),
			_ => left is null ? -1 : 1
		};


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(TreeNode<T>? left, TreeNode<T>? right) =>
			InternalCompare(left, right) == 0;

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(TreeNode<T>? left, TreeNode<T>? right) => !(left == right);

		/// <inheritdoc cref="Operators.operator &gt;"/>
		public static bool operator >(TreeNode<T>? left, TreeNode<T>? right) =>
			InternalCompare(left, right) > 0;

		/// <inheritdoc cref="Operators.operator &gt;="/>
		public static bool operator >=(TreeNode<T>? left, TreeNode<T>? right) =>
			InternalCompare(left, right) >= 0;

		/// <inheritdoc cref="Operators.operator &lt;"/>
		public static bool operator <(TreeNode<T>? left, TreeNode<T>? right) =>
			InternalCompare(left, right) < 0;

		/// <inheritdoc cref="Operators.operator &lt;="/>
		public static bool operator <=(TreeNode<T>? left, TreeNode<T>? right) =>
			InternalCompare(left, right) <= 0;
	}
}
