using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Indicates a chain node.
	/// </summary>
	/// <remarks>
	/// Here, <see cref="Node"/> uses a recursive data structure to store its
	/// parents. The algorithm to search chains uses <b>BFS</b> (i.e. breadth-first searching).
	/// </remarks>
	public sealed partial class Node : IEquatable<Node?>
	{
		/// <summary>
		/// Initializes an instance with the grid, candidate and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="candidate">Thecandidate.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Node(int candidate, bool isOn) => (Candidate, IsOn) = (candidate, isOn);

		/// <summary>
		/// Initializes an instance with the grid, candidate, a <see cref="bool"/> value
		/// and the node cause.
		/// </summary>
		/// <param name="candidate">Thecandidate.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		/// <param name="nodeCause">The node cause.</param>
		/// <param name="explanation">The explanation.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Node(int candidate, bool isOn, Cause? nodeCause, string? explanation) : this(candidate, isOn) =>
			(NodeCause, Explanation) = (nodeCause, explanation);

		/// <summary>
		/// Initializes an instance with the grid, candidate, a <see cref="bool"/> value,
		/// the node cause and the nested chain hint.
		/// </summary>
		/// <param name="candidate">Thecandidate.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		/// <param name="nodeCause">The node cause.</param>
		/// <param name="nestedChain">The nested chain.</param>
		/// <param name="explanation">The explanation.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Node(int candidate, bool isOn, Cause? nodeCause, ChainingTechniqueInfo nestedChain, string? explanation)
			: this(candidate, isOn, nodeCause, explanation) => NestedChain = nestedChain;

		/// <summary>
		/// Initializes an instance with the grid, candidate, a <see cref="bool"/> value,
		/// the node cause and the nested chain hint.
		/// </summary>
		/// <param name="candidate">Thecandidate.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		/// <param name="nodeCause">The node cause.</param>
		/// <param name="parent">The parent node.</param>
		/// <param name="explanation">Teh explanation.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Node(int candidate, bool isOn, Cause? nodeCause, Node parent, string? explanation)
			: this(candidate, isOn, nodeCause, explanation) => Parents.Add(parent);


		/// <summary>
		/// Indicates whether the node is on.
		/// </summary>
		public bool IsOn { get; }

		/// <summary>
		/// Indicates the candidate value.
		/// </summary>
		public int Candidate { get; }

		/// <summary>
		/// Indictaes the explanation.
		/// </summary>
		public string? Explanation { get; }

		/// <summary>
		/// The cause of this node.
		/// </summary>
		public Cause? NodeCause { get; }

		/// <summary>
		/// The nested chain information.
		/// </summary>
		public ChainingTechniqueInfo? NestedChain { get; }

		/// <summary>
		/// Indicates the parents node.
		/// </summary>
		public IList<Node> Parents { get; } = new List<Node>(1);


		/// <summary>
		/// Remove the candidate from the grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Off(Grid grid) => grid[Candidate / 9, Candidate % 9] = true;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Node? other) => Equals(this, other);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => Candidate << 1 + (IsOn ? 1 : 0);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => new CandidateCollection(Candidate).ToString();

		/// <summary>
		/// Converts the instance to <see cref="string"/> with the specified link type.
		/// </summary>
		/// <param name="linkType">The link type.</param>
		/// <returns>The <see cref="string"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(LinkType linkType)
		{
			string cellStr = new CellCollection(Candidate / 9).ToString();
			int digit = Candidate % 9 + 1;
			return linkType switch
			{
				LinkType.Strong => $"{cellStr}{(IsOn ? " must contain " : " cannot contain ")}{digit}",
				LinkType.Weak => $"{cellStr}{(IsOn ? " contains " : "doesn't contain ")}{digit}",
				_ => string.Empty
			};
		}


		/// <summary>
		/// To determine whether two nodes hold the same value.
		/// </summary>
		/// <param name="this">The left node.</param>
		/// <param name="other">The right node.</param>
		/// <returns>The return value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Equals(Node? @this, Node? other) =>
			(@this is null, other is null) switch
			{
				(true, true) => true,
				(false, false) => (@this!.Candidate, @this.IsOn) == (other!.Candidate, other.IsOn),
				_ => false
			};


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Node? left, Node? right) => Equals(left, right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Node? left, Node? right) => !(left == right);
	}
}
