using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
	/// <summary>
	/// Encapsulates a tree node.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	public sealed class TreeNode<T> : IComparable<TreeNode<T>?>
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
		/// Indicates the content.
		/// </summary>
		[MaybeNull]
		public T Content { get; set; } = default!;

		/// <summary>
		/// Indicates its children nodes.
		/// </summary>
		public ICollection<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();


		/// <inheritdoc/>
		public int CompareTo(TreeNode<T>? other) =>
			(this is null, other is null) switch
			{
				(true, true) => 0,
				(false, false) => Id.CompareTo(other!.Id),
				_ => this is null ? -1 : 1
			};

		/// <inheritdoc/>
		public override string ToString() =>
			$"{{Id = {Id}, ParentId = {ParentId}, Content = {Content}, ChildrenCount = {Children.Count}}}";
	}
}
