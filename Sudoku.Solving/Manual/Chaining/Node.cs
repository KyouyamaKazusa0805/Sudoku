using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides an elementary unit in a chain.
	/// </summary>
	public abstract class Node : IEquatable<Node>
	{
		/// <summary>
		/// Initializes an instance with the specified candidates and
		/// a <see cref="bool"/> value.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		protected Node(IEnumerable<int> candidates) : this(new FullGridMap(candidates))
		{ 
		}

		/// <summary>
		/// Initializes an instance with the specified map.
		/// </summary>
		/// <param name="candidates">The map of candidates.</param>
		protected Node(FullGridMap candidates) => Candidates = candidates;


		/// <summary>
		/// Indicates all candidates used in this node.
		/// </summary>
		public FullGridMap Candidates { get; }

		/// <summary>
		/// Indicates the type of this current node.
		/// </summary>
		public abstract NodeType NodeType { get; }


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		/// <param name="nodeType">
		/// (<see langword="out"/> parameter) Indicates the node type.
		/// </param>
		public void Deconstruct(out FullGridMap map, out NodeType nodeType) =>
			(map, nodeType) = (Candidates, NodeType);

		/// <inheritdoc/>
		public sealed override bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		public virtual bool Equals(Node other) => Candidates == other.Candidates;

		/// <inheritdoc/>
		/// <remarks>
		/// If you get a derived class, we recommend you override this method
		/// to describe the type of the node.
		/// </remarks>
		public override int GetHashCode() => Candidates.GetHashCode();

		/// <inheritdoc/>
		/// <remarks>
		/// If you get a derived class, we recommend you override this method
		/// to describe the type of the node.
		/// </remarks>
		public override string ToString() =>
			CandidateCollection.ToString(Candidates.Offsets);


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Node left, Node right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Node left, Node right) => !(left == right);
	}
}
