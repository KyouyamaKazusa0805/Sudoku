using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Encapsulates a <b>uniqueness square</b> (US) technique searcher.
	/// </summary>
	[TechniqueDisplay("Unique Square")]
	public sealed partial class UsTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates the patterns.
		/// </summary>
		private static readonly GridMap[] Patterns = new GridMap[162];


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 53;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			foreach (var pattern in Patterns)
			{
				if ((EmptyMap | pattern) != EmptyMap)
				{
					continue;
				}

				short mask = 0;
				foreach (int cell in pattern)
				{
					mask |= grid.GetCandidateMask(cell);
				}

				CheckType1(accumulator, grid, pattern, mask);
				CheckType2(accumulator, pattern, mask);
			}
		}

		private void CheckType1(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap pattern, short mask)
		{
			if (mask.CountSet() != 5)
			{
				return;
			}

			foreach (int[] digits in GetCombinationsOfArray(mask.GetAllSets().ToArray(), 4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				int extraDigit = (mask & ~digitsMask).FindFirstSet();
				var extraDigitMap = CandMaps[extraDigit] & pattern;
				if (extraDigitMap.Count != 1)
				{
					continue;
				}

				int elimCell = extraDigitMap.SetAt(0);
				short cellMask = grid.GetCandidateMask(elimCell);
				short elimMask = (short)(cellMask & ~(1 << extraDigit));
				if (elimMask == 0)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int digit in elimMask.GetAllSets())
				{
					conclusions.Add(new Conclusion(Elimination, elimCell, digit));
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int digit in digits)
				{
					foreach (int cell in pattern - elimCell & CandMaps[digit])
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new UsType1TechniqueInfo(
						conclusions,
						views: new[] { new View(candidateOffsets) },
						cells: pattern,
						digitsMask,
						candidate: elimCell * 9 + extraDigit));
			}
		}

		private void CheckType2(IBag<TechniqueInfo> accumulator, GridMap pattern, short mask)
		{
			if (mask.CountSet() != 5)
			{
				return;
			}

			foreach (int[] digits in GetCombinationsOfArray(mask.GetAllSets().ToArray(), 4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				int extraDigit = (mask & ~digitsMask).FindFirstSet();
				var elimMap = (CandMaps[extraDigit] & pattern).PeerIntersection & CandMaps[extraDigit];
				if (elimMap.IsEmpty)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int digit in digits)
				{
					foreach (int cell in CandMaps[digit] & pattern)
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}
				foreach (int cell in CandMaps[extraDigit] & pattern)
				{
					candidateOffsets.Add((1, cell * 9 + extraDigit));
				}

				accumulator.Add(
					new UsType2TechniqueInfo(
						conclusions,
						views: new[] { new View(candidateOffsets) },
						cells: pattern,
						digitsMask,
						extraDigit));
			}
		}
	}
}
