using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Data
{
	/// <summary>
	/// Provides an elementary unit in a chain.
	/// </summary>
	public sealed class Node : IComparable<Node>, IEquatable<Node>
	{
		/// <summary>
		/// Initializes an instance with a specified candidate and its type.
		/// </summary>
		/// <param name="candidate">The candidates.</param>
		/// <param name="nodeType">The type of this node.</param>
		public Node(int candidate, NodeType nodeType)
			: this(new FullGridMap(stackalloc[] { candidate }), nodeType)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified candidates and its type.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		/// <param name="nodeType">The type of this node.</param>
		public Node(IEnumerable<int> candidates, NodeType nodeType)
			: this(new FullGridMap(candidates), nodeType)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified map and its type.
		/// </summary>
		/// <param name="candidatesMap">The map of candidates.</param>
		/// <param name="nodeType">The node type.</param>
		public Node(FullGridMap candidatesMap, NodeType nodeType) =>
			(CandidatesMap, NodeType) = (candidatesMap, nodeType);


		/// <summary>
		/// Indicates all candidates used in this node.
		/// </summary>
		public FullGridMap CandidatesMap { get; }

		/// <summary>
		/// Indicates the type of this current node.
		/// </summary>
		public NodeType NodeType { get; }

		/// <summary>
		/// Indicates all candidates used.
		/// </summary>
		public IEnumerable<int> Candidates => CandidatesMap.Offsets;


		/// <summary>
		/// Get a candidate offset in the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The candidate offset.</returns>
		public int this[int index] => CandidatesMap.SetAt(index);

		/// <summary>
		/// Get a candidate offset in the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The candidate offset.</returns>
		public int this[Index index] => CandidatesMap.SetAt(index);


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		/// <param name="nodeType">
		/// (<see langword="out"/> parameter) Indicates the node type.
		/// </param>
		public void Deconstruct(out FullGridMap map, out NodeType nodeType) =>
			(map, nodeType) = (CandidatesMap, NodeType);

		/// <summary>
		/// Checks whether all candidates used in this instance is collide with
		/// the other one. If two candidates hold at least one same candidate,
		/// we will say the node is collide with the other node (or reversely).
		/// </summary>
		/// <param name="other">The other node.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public bool IsCollideWith(Node other) => (this & other).IsNotEmpty;

		/// <summary>
		/// Checks whether all candidates used in this instance fully covered
		/// the other one.
		/// </summary>
		/// <param name="other">The other node.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public bool FullCovered(Node other) =>
			CandidatesMap.Count < other.CandidatesMap.Count ? false : (this | other) == CandidatesMap;

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Node other) => CandidatesMap == other.CandidatesMap;

		/// <inheritdoc/>
		/// <remarks>
		/// If you get a derived class, we recommend you override this method
		/// to describe the type of the node.
		/// </remarks>
		public override int GetHashCode() => CandidatesMap.GetHashCode();

		/// <inheritdoc/>
		public int CompareTo(Node other) => this[0].CompareTo(other[^1]);

		/// <inheritdoc/>
		/// <remarks>
		/// If you get a derived class, we recommend you override this method
		/// to describe the type of the node.
		/// </remarks>
		public override string ToString()
		{
			const string separator = ", ";
			var sb = new StringBuilder();
			foreach (var candidateGroupByDigit in
				from candidate in from cand in CandidatesMap.Offsets orderby cand select cand
				group candidate by candidate % 9)
			{
				int digit = candidateGroupByDigit.Key;
				var group = from candidate in candidateGroupByDigit
							group candidate by candidate / 81;
				int cellGroupCount = group.Count();
				if (cellGroupCount >= 2)
				{
					sb.Append("{ ");
				}
				foreach (var candidateGroupByCellRow in group)
				{
					int cellRow = candidateGroupByCellRow.Key;
					sb.Append($"r{cellRow + 1}c");
					foreach (int cell in candidateGroupByCellRow)
					{
						sb.Append($"{cell / 9 % 9 + 1}");
					}

					sb.Append(separator);
				}

				sb.RemoveFromEnd(separator.Length);
				if (cellGroupCount >= 2)
				{
					sb.Append(" }");
				}
				sb.Append($"({digit + 1}){separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Node left, Node right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Node left, Node right) => !(left == right);

		/// <summary>
		/// Check whether the <paramref name="left"/> node is greater than <paramref name="right"/>
		/// node.
		/// </summary>
		/// <param name="left">The left node.</param>
		/// <param name="right">The right node.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		public static bool operator >(Node left, Node right) => left.CompareTo(right) > 0;

		/// <summary>
		/// Check whether the <paramref name="left"/> node is greater than
		/// or equals to <paramref name="right"/> node.
		/// </summary>
		/// <param name="left">The left node.</param>
		/// <param name="right">The right node.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		public static bool operator >=(Node left, Node right) => left.CompareTo(right) >= 0;

		/// <summary>
		/// Check whether the <paramref name="left"/> node is less than <paramref name="right"/>
		/// node.
		/// </summary>
		/// <param name="left">The left node.</param>
		/// <param name="right">The right node.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		public static bool operator <(Node left, Node right) => left.CompareTo(right) < 0;

		/// <summary>
		/// Check whether the <paramref name="left"/> node is less than or equals to
		/// <paramref name="right"/> node.
		/// </summary>
		/// <param name="left">The left node.</param>
		/// <param name="right">The right node.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		public static bool operator <=(Node left, Node right) => left.CompareTo(right) <= 0;

		/// <summary>
		/// Get all candidates that <paramref name="left"/> and <paramref name="right"/>
		/// map both contain.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>All candidates that satisfied the condition.</returns>
		public static FullGridMap operator &(Node left, Node right) =>
			left.CandidatesMap & right.CandidatesMap;

		/// <summary>
		/// Get all candidates from <paramref name="left"/> and <paramref name="right"/>
		/// maps.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>All candidates.</returns>
		public static FullGridMap operator |(Node left, Node right) =>
			left.CandidatesMap | right.CandidatesMap;

		/// <summary>
		/// Get all candidates that satisfy the formula <c>(a - b) | (b - a)</c>.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>All candidates.</returns>
		public static FullGridMap operator ^(Node left, Node right) =>
			left.CandidatesMap ^ right.CandidatesMap;

		/// <summary>
		/// Get all candidates that is in the <paramref name="left"/> map but not in
		/// the <paramref name="right"/> map (i.e. formula <c>a &#38; ~b</c>).
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>All candidates.</returns>
		public static FullGridMap operator -(Node left, Node right) =>
			left.CandidatesMap - right.CandidatesMap;
	}
}
