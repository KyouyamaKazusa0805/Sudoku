using System;

namespace Sudoku.Solving.Chaining
{
	/// <summary>
	/// Provides a relationship of two <see cref="Node"/>s.
	/// </summary>
	/// <seealso cref="Node"/>
	public readonly struct Inference : IEquatable<Inference>
	{
		/// <summary>
		/// Initializes an instance with the specified nodes.
		/// </summary>
		/// <param name="startCell">The start cell offset.</param>
		/// <param name="startCellIsOn">Indicates whether the start cell is on.</param>
		/// <param name="endCell">The end cell offset.</param>
		/// <param name="endCellIsOn">Indicates whether the end cell is on.</param>
		public Inference(int startCell, bool startCellIsOn, int endCell, bool endCellIsOn) =>
			(StartNode, EndNode) = (new Node(startCell, startCellIsOn), new Node(endCell, endCellIsOn));

		/// <summary>
		/// Initializes an instance with two nodes.
		/// </summary>
		/// <param name="startNode">The start node.</param>
		/// <param name="endNode">The end node.</param>
		public Inference(Node startNode, Node endNode) => (StartNode, EndNode) = (startNode, endNode);


		/// <summary>
		/// The start node.
		/// </summary>
		public Node StartNode { get; }

		/// <summary>
		/// The end node.
		/// </summary>
		public Node EndNode { get; }


		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Inference comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Inference other)
		{
			int s1 = StartNode.GetHashCode();
			int s2 = other.StartNode.GetHashCode();
			int e1 = EndNode.GetHashCode();
			int e2 = other.EndNode.GetHashCode();
			return s1 - s2 == 0 && e1 - e2 == 0 || s1 + e2 == 0 && s2 + e1 == 0;
		}

		/// <inheritdoc/>
		public override int GetHashCode() => StartNode.GetHashCode() ^ EndNode.GetHashCode();

		/// <inheritdoc/>
		public override string ToString() => $"{StartNode} -> {EndNode}";



		/// <include file='../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Inference left, Inference right) => left.Equals(right);

		/// <include file='../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Inference left, Inference right) => !(left == right);
	}
}
