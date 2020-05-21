using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sudoku.Data
{
	/// <summary>
	/// Provides an inference between two nodes.
	/// </summary>
	/// <remarks>
	/// This data structure is so heavy...
	/// </remarks>
	public sealed class Inference : IEquatable<Inference?>
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
		public bool IsStrong => !StartIsOn && EndIsOn;

		/// <summary>
		/// Indicates whether the inference is weak.
		/// </summary>
		public bool IsWeak => StartIsOn && !EndIsOn;

		/// <summary>
		/// Indicates the intersection of the current inference, which is used
		/// in searching for eliminations in loops or normal AICs.
		/// </summary>
		public FullGridMap Intersection
		{
			get
			{
				switch (Start.NodeType)
				{
					case NodeType.Candidate:
					{
						switch (End.NodeType)
						{
							case NodeType.Candidate:
							case NodeType.LockedCandidates:
							{
								return FullGridMap.CreateInstance(Start.Candidates.Concat(End.Candidates));
							}
						}

						break;
					}
					case NodeType.LockedCandidates:
					{
						switch (End.NodeType)
						{
							case NodeType.Candidate:
							case NodeType.LockedCandidates:
							{
								return FullGridMap.CreateInstance(Start.Candidates.Concat(End.Candidates));
							}
						}

						break;
					}
				}

				return FullGridMap.Empty;
			}
		}


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		public void Deconstruct(out Node start, out Node end) => (start, end) = (Start, End);

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
			(startMap, startInOn, startNodeType, endMap, endIsOn, endNodeType) = (Start.CandidatesMap, StartIsOn, Start.NodeType, End.CandidatesMap, EndIsOn, End.NodeType);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Inference comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Inference? other) =>
			(this is null, other is null) switch
			{
				(true, true) => true,
				(false, false) => Start == other!.Start && End == other.End,
				_ => false
			};

		/// <inheritdoc/>
		public override int GetHashCode() => Start.GetHashCode() ^ End.GetHashCode();

		/// <inheritdoc/>
		public override string ToString() => $"{Sgn(StartIsOn)}{Start} -> {Sgn(EndIsOn)}{End}";

		/// <summary>
		/// The sign function used in <see cref="ToString"/>.
		/// </summary>
		/// <param name="v">The value.</param>
		/// <returns>The string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string Sgn(bool v) => v ? string.Empty : "!";


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Inference left, Inference right) => left.Equals(right);
		
		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Inference left, Inference right) => !(left == right);
	}
}
