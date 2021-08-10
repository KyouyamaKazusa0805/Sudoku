using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates a <b>locked candidates</b> (LC) technique searcher.
	/// </summary>
	public sealed class LcStepSearcher : IntersectionStepSearcher
	{
		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(2, DisplayingLevel: DisplayingLevel.A);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(2, nameof(Technique.Pointing))
		{
			DisplayLevel = 1
		};


		/// <inheritdoc/>
		public override unsafe void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Here we'll use a newer algorithm to check it.
			// .-------.-------.-------.
			// | C C C | A A A | A A A |
			// | B B B | . . . | . . . |
			// | B B B | . . . | . . . |
			// '-------'-------'-------'
			// For example, if the cells C form a locked candidates, there'll be two cases:
			// 1) Pointing (Type 1): Cells A contains the digit, but cells B doesn't.
			// 2) Claiming (Type 2): Cells B contains the digit, but cells A doesn't.
			// So, the algorithm is:
			// Use bitwise or operator to gather all candidate masks from cells A, cells B and cells C,
			// and suppose the notation 'a' is the mask result for cells A, 'b' is the mask result for cells B,
			// and 'c' is the mask result for cells C. If the equation is correct:
			//
			// c & (a ^ b) != 0
			//
			// The locked candidates exists, and the result of the expression 'c & (a ^ b)' is a mask
			// that holds the digits of the locked candidates.
			// Why this expression? 'a ^ b' means the digit can only appear in either cells A or cells B.
			// If both or neither, the digit won't contain the locked candidates structure.
			// Because of the optimization of the performance, we use the predefined table to iterate on
			// all possible location where may form a locked candidate.
			int* r = stackalloc int[2];
			foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
			{
				// If the cells C doesn't contain any empty cells,
				// the location won't contain any locked candidates.
				if ((EmptyMap & c).IsEmpty)
				{
					continue;
				}

				// Gather the masks in cells A, B and C.
				short maskA = 0, maskB = 0, maskC = 0;
				foreach (int cell in a) maskA |= grid.GetCandidates(cell);
				foreach (int cell in b) maskB |= grid.GetCandidates(cell);
				foreach (int cell in c) maskC |= grid.GetCandidates(cell);

				// Use the formula, and check whether the equation is correct.
				// If so, the mask 'm' will hold the digits that form locked candidates structures.
				short m = (short)(maskC & (maskA ^ maskB));
				if (m == 0)
				{
					continue;
				}

				// Now iterate on the mask to get all digits.
				foreach (int digit in m)
				{
					// Check whether the digit contains any eliminations.
					Cells elimMap;
					if (!(a & CandMaps[digit]).IsEmpty)
					{
						r[0] = coverSet;
						r[1] = baseSet;
						elimMap = a & CandMaps[digit];
					}
					else
					{
						r[0] = baseSet;
						r[1] = coverSet;
						elimMap = b & CandMaps[digit];
					}
					if (elimMap.IsEmpty)
					{
						continue;
					}

					// Gather the information,
					// such as the type of the locked candidates, the located region, etc..
					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, digit));
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in c & CandMaps[digit])
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}

					// Okay, now accumulate into the collection.
					accumulator.Add(
						new LcStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, r[0]), new(2, r[1]) }
								}
							},
							digit,
							r[0],
							r[1]
						)
					);
				}
			}
		}
	}
}
