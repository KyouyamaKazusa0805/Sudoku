using System;

namespace Sudoku.Data
{
	/// <summary>
	/// Provides a relationship of two <see cref="ChainNode"/>s.
	/// This data structure is used for searching for AICs.
	/// </summary>
	/// <seealso cref="ChainNode"/>
	public readonly struct ChainInference : IEquatable<ChainInference>
	{
		/// <summary>
		/// Initializes an instance with the specified nodes.
		/// </summary>
		/// <param name="startCell">The start cell offset.</param>
		/// <param name="startCellIsOn">Indicates whether the start cell is on.</param>
		/// <param name="endCell">The end cell offset.</param>
		/// <param name="endCellIsOn">Indicates whether the end cell is on.</param>
		public ChainInference(int startCell, bool startCellIsOn, int endCell, bool endCellIsOn) =>
			(StartNode, EndNode) = (new ChainNode(startCell, startCellIsOn), new ChainNode(endCell, endCellIsOn));

		/// <summary>
		/// Initializes an instance with two nodes.
		/// </summary>
		/// <param name="startNode">The start node.</param>
		/// <param name="endNode">The end node.</param>
		public ChainInference(ChainNode startNode, ChainNode endNode) => (StartNode, EndNode) = (startNode, endNode);


		/// <summary>
		/// Indicates whether the start candidate is on.
		/// </summary>
		public bool StartIsOn => StartNode.IsOn;

		/// <summary>
		/// Indicates whether the end candidate is on.
		/// </summary>
		public bool EndIsOn => EndNode.IsOn;

		/// <summary>
		/// Indicates the start cell.
		/// </summary>
		public int StartCell => StartNode.Candidate / 9;

		/// <summary>
		/// Indicates the start digit.
		/// </summary>
		public int StartDigit => StartNode.Candidate % 9;

		/// <summary>
		/// Indicates the end cell.
		/// </summary>
		public int EndCell => EndNode.Candidate / 9;

		/// <summary>
		/// Indicates the end digit.
		/// </summary>
		public int EndDigit => EndNode.Candidate % 9;

		/// <summary>
		/// Indicates the start candidate.
		/// </summary>
		public int StartCandidate => StartNode.Candidate;

		/// <summary>
		/// Indicates the end candidate.
		/// </summary>
		public int EndCandidate => EndNode.Candidate;

		/// <summary>
		/// The start node.
		/// </summary>
		public ChainNode StartNode { get; }

		/// <summary>
		/// The end node.
		/// </summary>
		public ChainNode EndNode { get; }


		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		public void Deconstruct(out ChainNode start, out ChainNode end) =>
			(start, end) = (StartNode, EndNode);

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="startCand">(<see langword="out"/> parameter) The start candidate.</param>
		/// <param name="startIsOn">
		/// (<see langword="out"/> parameter) Indicates whether the start candidate is on.
		/// </param>
		/// <param name="endCand">(<see langword="out"/> parameter) The end candidate.</param>
		/// <param name="endIsOn">
		/// (<see langword="out"/> parameter) Indicates whether the end candidate is on.
		/// </param>
		public void Deconstruct(
			out int startCand, out bool startIsOn, out int endCand, out bool endIsOn) =>
			(startCand, startIsOn, endCand, endIsOn) = (StartCandidate, StartIsOn, EndCandidate, EndIsOn);

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="startCell">(<see langword="out"/> parameter) The start cell.</param>
		/// <param name="startDigit">(<see langword="out"/> parameter) The start digit.</param>
		/// <param name="startIsOn">
		/// (<see langword="out"/> parameter) Indicates whether the start candidate is on.
		/// </param>
		/// <param name="endCell">(<see langword="out"/> parameter) The end cell.</param>
		/// <param name="endDigit">(<see langword="out"/> parameter) The end digit.</param>
		/// <param name="endIsOn">
		/// (<see langword="out"/> parameter) Indicates whether the end candidate is on.
		/// </param>
		public void Deconstruct(
			out int startCell, out int startDigit, out bool startIsOn,
			out int endCell, out int endDigit, out bool endIsOn) =>
			(startCell, startDigit, startIsOn, endCell, endDigit, endIsOn) = (StartCell, StartDigit, StartIsOn, EndCell, EndDigit, EndIsOn);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is ChainInference comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(ChainInference other)
		{
			int s1 = StartNode.GetHashCode();
			int s2 = other.StartNode.GetHashCode();
			int e1 = EndNode.GetHashCode();
			int e2 = other.EndNode.GetHashCode();
			return s1 - s2 == 0 && e1 - e2 == 0 || s1 + e2 == 0 && s2 + e1 == 0;
		}

		/// <inheritdoc/>
		public override int GetHashCode() => StartNode.GetHashCode() ^ EndNode.GetHashCode();

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => $"{StartNode} -> {EndNode}";


		/// <include file='../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ChainInference left, ChainInference right) => left.Equals(right);

		/// <include file='../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ChainInference left, ChainInference right) => !(left == right);
	}
}
