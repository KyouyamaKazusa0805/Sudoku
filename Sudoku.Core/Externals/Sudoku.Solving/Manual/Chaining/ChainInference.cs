using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a chain inference.
	/// </summary>
	public readonly struct ChainInference
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="start">The start node.</param>
		/// <param name="end">The end node.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChainInference(ChainNode start, ChainNode end) : this(start, false, end, true, ChainNodeType.Candidate)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="start">The start node.</param>
		/// <param name="startIsOn">Indicates whether the start node is on.</param>
		/// <param name="end">The end node.</param>
		/// <param name="endIsOn">Indicates whether the end node is on.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChainInference(ChainNode start, bool startIsOn, ChainNode end, bool endIsOn)
			: this(start, startIsOn, end, endIsOn, ChainNodeType.Candidate)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="start">The start node.</param>
		/// <param name="startIsOn">Indicates whether the start node is on.</param>
		/// <param name="end">The end node.</param>
		/// <param name="endIsOn">Indicates whether the end node is on.</param>
		/// <param name="nodeType">The node type.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChainInference(ChainNode start, bool startIsOn, ChainNode end, bool endIsOn, ChainNodeType nodeType) =>
			(Start, StartIsOn, End, EndIsOn, NodeType) = (start, startIsOn, end, endIsOn, nodeType);


		/// <summary>
		/// Indicates whether the start node is on.
		/// </summary>
		public bool StartIsOn { get; }

		/// <summary>
		/// Indicates whether the end node is on.
		/// </summary>
		public bool EndIsOn { get; }

		/// <summary>
		/// Indicates the start node.
		/// </summary>
		public ChainNode Start { get; }

		/// <summary>
		/// Indicates the end node.
		/// </summary>
		public ChainNode End { get; }

		/// <summary>
		/// Indicates the node type.
		/// </summary>
		public ChainNodeType NodeType { get; }


		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		public void Deconstruct(out ChainNode start, out ChainNode end) => (start, end) = (Start, End);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		/// <param name="nodeType">(<see langword="out"/> parameter) The node type.</param>
		public void Deconstruct(out ChainNode start, out ChainNode end, out ChainNodeType nodeType) =>
			(start, end, nodeType) = (Start, End, NodeType);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="startIsOn">(<see langword="out"/> parameter) Whether the start node is on.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		/// <param name="endIsOn">(<see langword="out"/> parameter) Whether the end node is on.</param>
		public void Deconstruct(out ChainNode start, out bool startIsOn, out ChainNode end, out bool endIsOn) =>
			(start, startIsOn, end, endIsOn) = (Start, StartIsOn, End, EndIsOn);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="startIsOn">(<see langword="out"/> parameter) Whether the start node is on.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		/// <param name="endIsOn">(<see langword="out"/> parameter) Whether the end node is on.</param>
		/// <param name="nodeType">(<see langword="out"/> parameter) The node type.</param>
		public void Deconstruct(
			out ChainNode start, out bool startIsOn, out ChainNode end, out bool endIsOn, out ChainNodeType nodeType) =>
			(start, startIsOn, end, endIsOn, nodeType) = (Start, StartIsOn, End, EndIsOn, NodeType);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			var ((d1, map1), b1, (d2, map2), b2) = this;
			string linkOperator = (b1, b2) switch
			{
				(true, true) => " => ",
				(true, false) => " -- ",
				(false, true) => " == ",
				(false, false) => " -> "
			};

			string cells1 = new CellCollection(map1).ToString();
			string cells2 = new CellCollection(map2).ToString();
			return d1 == d2
				? $"{cells1}{linkOperator}{cells2}({d1 + 1})"
				: $"{cells1}({d1 + 1}){linkOperator}{cells2}({d2 + 1})";
		}
	}
}
