using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>alternating inference chain</b> (AIC) technique searcher.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This technique searcher may use the basic searching way to find all AICs and
	/// grouped AICs. For example, this searcher will try to search for all strong
	/// inferences firstly, and then search a weak inference that the candidate is
	/// in the same region or cell with a node in the strong inference in order to
	/// link them.
	/// </para>
	/// <para>
	/// Note that AIC may be static chains, which means that the searcher may just use
	/// static analysis is fine, which is different with dynamic chains.
	/// </para>
	/// <para>By the way, the searching method is <b>BFS</b> (breadth-first searching).</para>
	/// </remarks>
	[TechniqueDisplay(nameof(TechniqueCode.Aic))]
	public sealed class AicTechniqueSearcher : ChainTechniqueSearcher
	{
		/// <summary>
		/// Indicates the maximum length to search.
		/// </summary>
		private readonly int _maxLength;


		/// <summary>
		/// Initializes an instance with the specified length.
		/// </summary>
		/// <param name="maxLength">The max length.</param>
		public AicTechniqueSearcher(int maxLength) => _maxLength = maxLength;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 50;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var list = new ICollection<Inference>?[81];
			GetStrongRelations(list, grid);

			// The temp accumulator.
			var tempAccumulator = new Bag<TechniqueInfo>();

			// Iterate on each cell.
			var fullMap = new SudokuMap();
			var stack = new List<Inference>();
			for (int cell = 0; cell < 81; cell++)
			{
				var inferencesGroupedByCell = list[cell];
				if (inferencesGroupedByCell is null)
				{
					continue;
				}

				// Iterate on each inference.
				foreach (var inference in inferencesGroupedByCell)
				{
					var ((startDigit, startMap), (endDigit, endMap)) = inference;
					int startCandidate = startMap.SetAt(0) * 9 + startDigit, endCandidate = endMap.SetAt(0) * 9 + endDigit;
					fullMap.Add(startCandidate);
					fullMap.Add(endCandidate);
					stack.Add(inference);

					GetOnToOff(tempAccumulator, grid, list, _maxLength - 1, stack, fullMap, endCandidate);

					stack.RemoveLastElement();
					fullMap.Remove(startCandidate);
					fullMap.Remove(endCandidate);
				}
			}

			// TODO: Sort the accumulator.
		}

		/// <summary>
		/// Get off to on (search for strong links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="list">The list.</param>
		/// <param name="length">The length.</param>
		/// <param name="stack">The temporary stack.</param>
		/// <param name="fullMap">The map of candidates that currently used.</param>
		/// <param name="candidate">The candidate.</param>
		private void GetOffToOn(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, ICollection<Inference>?[] list,
			int length, IList<Inference> stack, SudokuMap fullMap, int candidate)
		{
			if (length <= 0)
			{
				return;
			}

			byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);
			var inferences = list[cell];
			if (inferences is null)
			{
				// Failed to search.
				return;
			}

			// Iterate on each inference.
			foreach (var inference in inferences)
			{
				var ((startDigit, startMap), (endDigit, endMap)) = inference;
				int currentCandidate = endMap.SetAt(0) * 9 + endDigit;
				bool start = startMap[cell] && digit == startDigit, end = endMap[cell] && digit == endDigit;
				if (!start && !end)
				{
					continue;
				}

				if (fullMap[currentCandidate])
				{
					continue;
				}

				fullMap.Add(currentCandidate);
				stack.Add(start ? inference : new Inference(inference.End, inference.Start));

				// Because of the formed chain, we should check eliminations first.
				CheckEliminations(accumulator, grid, stack);

				// Then get on to off recursively.
				GetOnToOff(accumulator, grid, list, length - 1, stack, fullMap, currentCandidate);

				stack.RemoveLastElement();
				fullMap.Remove(currentCandidate);
			}
		}

		/// <summary>
		/// Get on to off (search for weak links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="list">The list.</param>
		/// <param name="length">The length.</param>
		/// <param name="stack">The temporary stack.</param>
		/// <param name="fullMap">The map of candidates that currently used.</param>
		/// <param name="candidate">The candidate.</param>
		private void GetOnToOff(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, ICollection<Inference>?[] list,
			int length, IList<Inference> stack, SudokuMap fullMap, int candidate)
		{
			if (length <= 0)
			{
				return;
			}

			byte digit = (byte)(candidate % 9), cell = (byte)(candidate / 9);

			// Iterate on other candidates.
			foreach (byte d in grid.GetCandidates(cell))
			{
				if (d == digit)
				{
					continue;
				}

				int currentCandidate = cell * 9 + d;
				if (fullMap[currentCandidate])
				{
					continue;
				}

				fullMap.Add(currentCandidate);
				stack.Add(new Inference(new Node(candidate), true, new Node(currentCandidate), false));

				GetOffToOn(accumulator, grid, list, length - 1, stack, fullMap, currentCandidate);

				stack.RemoveLastElement();
				fullMap.Remove(currentCandidate);
			}

			// Iterate on other cells.
			foreach (byte c in PeerMaps[candidate / 9] & CandMaps[digit])
			{
				int currentCandidate = c * 9 + digit;
				if (fullMap[currentCandidate])
				{
					continue;
				}

				fullMap.Add(currentCandidate);
				stack.Add(new Inference(new Node(candidate), true, new Node(currentCandidate), false));

				GetOffToOn(accumulator, grid, list, length - 1, stack, fullMap, currentCandidate);

				stack.RemoveLastElement();
				fullMap.Remove(currentCandidate);
			}
		}

		/// <summary>
		/// Check eliminations.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="stack">The stack.</param>
		private void CheckEliminations(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, IList<Inference> stack)
		{
			//if (IsContinuousNiceLoop((List<Inference>)stack))
			//{
			//	var elimMap = new SudokuMap();
			//	for (int i = 1, count = stack.Count; i < count; i += 2)
			//	{
			//		elimMap |= stack[i].EliminationSet;
			//		// TODO: Remove redundant candidates that does not contain in the grid.
			//	}
			//}
			//else
			//{
			var (startDigit, startMap) = stack[0].Start;
			var (endDigit, endMap) = stack[^1].End;
			var elimMap =
				new SudokuMap(startMap.SetAt(0) * 9 + startDigit, false)
				& new SudokuMap(endMap.SetAt(0) * 9 + endDigit, false);
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int candidate in elimMap)
			{
				if (grid.Exists(candidate / 9, candidate % 9) is true)
				{
					conclusions.Add(new Conclusion(Elimination, candidate));
				}
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new List<(int, int)>();
			var ((sd, sm), startIsOn, (ed, em), endIsOn) = stack[0];
			int startCand = startMap.SetAt(0) * 9 + startDigit, endCand = endMap.SetAt(0) * 9 + endDigit;
			candidateOffsets.Add((startIsOn ? 1 : 0, startCand));
			candidateOffsets.Add((endIsOn ? 1 : 0, endCand));
			for (int i = 1, count = stack.Count; i < count; i++)
			{
				(_, _, (ed, em), endIsOn) = stack[i];
				candidateOffsets.Add((endIsOn ? 1 : 0, em.SetAt(0) * 9 + ed));
			}

			accumulator.Add(
				new AicTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: null,
							links: (List<Inference>)stack)
					},
					inferences: (List<Inference>)stack,
					isContinuousNiceLoop: false));
			//}
		}


		/// <summary>
		/// Get all strong relations at initial.
		/// </summary>
		/// <param name="list">The collected strong relations grouped by its cell.</param>
		/// <param name="grid">The grid.</param>
		private static void GetStrongRelations(ICollection<Inference>?[] list, IReadOnlyGrid grid)
		{
			foreach (byte cell in BivalueMap)
			{
				short mask = grid.GetCandidateMask(cell);
				byte d1 = (byte)mask.FindFirstSet();
				byte d2 = (byte)mask.GetNextSet(d1);
				list[cell] = new List<Inference>
				{
					new Inference(new Node(d1, cell), new Node(d2, cell))
				};
			}

			var regions = (Span<int>)stackalloc int[3];
			foreach (byte cell in EmptyMap - BivalueMap)
			{
				foreach (byte digit in grid.GetCandidates(cell))
				{
					regions[0] = GetRegion(cell, Row);
					regions[1] = GetRegion(cell, Column);
					regions[2] = GetRegion(cell, Block);
					foreach (int region in regions)
					{
						var map = CandMaps[digit] & RegionMaps[region];
						if (map.Count == 2)
						{
							// Conjugate pair (here will be strong link) found.
							(list[cell] ??= new List<Inference>()).Add(
								new Inference(
									new Node(digit, (byte)map.SetAt(0)),
									new Node(digit, (byte)map.SetAt(1))));
						}
					}
				}
			}
		}

		/// <summary>
		/// Determine whether the chain is a continuous nice loop.
		/// </summary>
		/// <param name="stack">The chain stack.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsContinuousNiceLoop(IReadOnlyList<Inference> stack)
		{
			var (startDigit, startMap) = stack[0].Start;
			var (endDigit, endMap) = stack[^1].End;
			int startCell = startMap.SetAt(0), endCell = endMap.SetAt(0);
			return startDigit == endDigit && new GridMap { startCell, endCell }.AllSetsAreInOneRegion(out _)
				|| startDigit != endDigit && startCell == endCell;
		}
	}
}
