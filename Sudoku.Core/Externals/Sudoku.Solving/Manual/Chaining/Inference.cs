using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a chain inference.
	/// </summary>
	public readonly struct Inference
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="start">The start node.</param>
		/// <param name="end">The end node.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Inference(Node start, Node end) : this(start, false, end, true)
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
		public Inference(Node start, bool startIsOn, Node end, bool endIsOn) =>
			(Start, StartIsOn, End, EndIsOn) = (start, startIsOn, end, endIsOn);


		/// <summary>
		/// Indicates whether the start node is on.
		/// </summary>
		public bool StartIsOn { get; }

		/// <summary>
		/// Indicates whether the end node is on.
		/// </summary>
		public bool EndIsOn { get; }

		/// <summary>
		/// Indicates whether the specified inference is a strong link.
		/// </summary>
		public bool IsStrong => !StartIsOn && EndIsOn;

		/// <summary>
		/// Indicates whether the specified inference is a weak inference.
		/// </summary>
		public bool IsWeak => StartIsOn && !EndIsOn;

		/// <summary>
		/// Indicates the start node.
		/// </summary>
		public Node Start { get; }

		/// <summary>
		/// Indicates the end node.
		/// </summary>
		public Node End { get; }

		/// <summary>
		/// Indicates the elimination set.
		/// </summary>
		public SudokuMap? EliminationSet
		{
			get
			{
				if (StartIsOn && !EndIsOn)
				{
					// Weak link.
					var (startDigit, startMap) = Start;
					var (endDigit, endMap) = End;
					int startCell = startMap.SetAt(0), endCell = endMap.SetAt(0);
					var twoCellsMap = new GridMap { startCell, endCell };
					if (startDigit == endDigit && twoCellsMap.AllSetsAreInOneRegion(out int region))
					{
						return new SudokuMap(RegionMaps[region] - twoCellsMap, startDigit);
					}
					else if (startDigit != endDigit && startCell == endCell)
					{
						var result = new SudokuMap(startCell);
						result.Remove(startCell * 9 + startDigit);
						result.Remove(endCell * 9 + endDigit);
						return result;
					}
					else
					{
						return null;
					}
				}

				return null;
			}
		}


		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		public void Deconstruct(out Node start, out Node end) => (start, end) = (Start, End);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="start">(<see langword="out"/> parameter) The start node.</param>
		/// <param name="startIsOn">(<see langword="out"/> parameter) Whether the start node is on.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end node.</param>
		/// <param name="endIsOn">(<see langword="out"/> parameter) Whether the end node is on.</param>
		public void Deconstruct(out Node start, out bool startIsOn, out Node end, out bool endIsOn) =>
			(start, startIsOn, end, endIsOn) = (Start, StartIsOn, End, EndIsOn);

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
