using System;
using Sudoku.Data;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides an inference between two nodes.
	/// </summary>
	public class Inference : IEquatable<Inference>
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="start">The start node.</param>
		/// <param name="startIsOn">Indicates whether the start node is on.</param>
		/// <param name="end">The end node.</param>
		/// <param name="endIsOn">Indicates whether the end node is on.</param>
		public Inference(Node start, bool startIsOn, Node end, bool endIsOn) =>
			(Start, StartIsOn, End, EndIsOn) = (start, startIsOn, end, endIsOn);


		/// <summary>
		/// Indicates the start node.
		/// </summary>
		public Node Start { get; }

		/// <summary>
		/// Indicates the end node.
		/// </summary>
		public Node End { get; }

		/// <summary>
		/// Indicates whether the start node is on.
		/// </summary>
		public bool StartIsOn { get; }

		/// <summary>
		/// Indicates whether the end node is on.
		/// </summary>
		public bool EndIsOn { get; }

		/// <summary>
		/// Indicates whether the inference is strong.
		/// </summary>
		public bool IsStrongInference => !StartIsOn && EndIsOn;

		/// <summary>
		/// Indicates whether the inference is weak.
		/// </summary>
		public bool IsWeakInference => StartIsOn && !EndIsOn;

		/// <summary>
		/// Indicates whether the inference is neither strong nor weak.
		/// </summary>
		public bool IsOtherInference => !IsStrongInference && !IsWeakInference;


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		public void Deconstruct(out Node start, out Node end) =>
			(start, end) = (Start, End);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="startMap">
		/// (<see langword="out"/> parameter) The candidates in start node.
		/// </param>
		/// <param name="startInOn">
		/// (<see langword="out"/> parameter) Indicates whether the start node is on.
		/// </param>
		/// <param name="startNodeType">
		/// (<see langword="out"/> parameter) Indicates the start node type.
		/// </param>
		/// <param name="endMap">
		/// (<see langword="out"/> parameter) The candidates in end node.
		/// </param>
		/// <param name="endIsOn">
		/// (<see langword="out"/> parameter) Indicates whether the end node is on.
		/// </param>
		/// <param name="endNodeType">
		/// (<see langword="out"/> parameter) Indicates the end node type.
		/// </param>
		public void Deconstruct(
			out FullGridMap startMap, out bool startInOn, out NodeType startNodeType,
			out FullGridMap endMap, out bool endIsOn, out NodeType endNodeType) =>
			(startMap, startInOn, startNodeType, endMap, endIsOn, endNodeType) = (Start.Candidates, StartIsOn, Start.NodeType, End.Candidates, EndIsOn, End.NodeType);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Inference comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Inference other) => Start == other.Start && End == other.End;

		/// <inheritdoc/>
		public override int GetHashCode() => Start.GetHashCode() ^ End.GetHashCode();

		/// <inheritdoc/>
		public override string ToString() => $"{Start} -> {End}";


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Inference left, Inference right) => left.Equals(right);
		
		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Inference left, Inference right) => !(left == right);
	}
}
