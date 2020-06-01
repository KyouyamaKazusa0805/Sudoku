namespace System.Collections
{
	/// <summary>
	/// Indicates a node used for dancing links.
	/// </summary>
	public unsafe struct DlxNode : IEquatable<DlxNode>
	{
		/// <summary>
		/// The pointer to the left node.
		/// </summary>
		public DlxNode* Left;

		/// <summary>
		/// The pointer to the right node.
		/// </summary>
		public DlxNode* Right;

		/// <summary>
		/// The pointer to the up node.
		/// </summary>
		public DlxNode* Up;

		/// <summary>
		/// The pointer to the right node.
		/// </summary>
		public DlxNode* Down;

		/// <summary>
		/// The pointer to the head node.
		/// </summary>
		public DlxNode* Column;

		/// <summary>
		/// The unique code for this node.
		/// </summary>
		public int Code;

		/// <summary>
		/// The size of the current column.
		/// </summary>
		public int Size;


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		public override bool Equals(object? obj) => obj is DlxNode comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(DlxNode other) => Code == other.Code;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override int GetHashCode() => Code;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() =>
			$"(u = {(int)Up:X}, d = {(int)Down:X}, l = {(int)Left:X}, r = {(int)Right:X}, id = {Code})";
	}
}
