using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates a <b>death blossom</b> technique.
	/// </summary>
	public sealed class DbStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(31, nameof(Technique.DeathBlossom))
		{
			DisplayLevel = 3,
			IsEnabled = false,
			DisabledReason = DisabledReason.HasBugs
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Create a copy.
			var readOnlyGrid = grid;

			// Gather all possible ALSes.
			var alses = Als.GetAllAlses(grid);

			// Now iterate on all candidates in the current grid.
			// In fact, start from version 0.2, you can write the code like:
			// foreach (int candidate in grid)
			// {
			//     // ...
			// }
			// Here we have got the empty map, so nested loop is faster.
			// Firstly, iterate on each empty cell.
			foreach (int cell in EmptyMap)
			{
				// Secondly, iterate on each digit in that cell.
				foreach (int digit in grid.GetCandidates(cell))
				{
					var relativeAlses = new List<Als>();

					// Get all ALSes relative to the current candidate.
					// Firstly, iterate on each ALS.
					foreach (var als in alses)
					{
						// Get all cells used in this ALS.
						var appearing = als.Map;
						appearing.RemoveAll(condition);

						// The method of that condition.
						// Why use local functions rather than lambdas? All captured variables
						// in local functions will be stored in a struct rather than a class.
						bool condition(int cell) => readOnlyGrid.Exists(cell, digit) is not true;

						// If the peer intersection contains that cell, the ALS is relative one.
						// Add into the list.
						if (appearing.PeerIntersection.Contains(cell))
						{
							relativeAlses.Add(als);
						}
					}

					for (int size = 2, maxSize = Math.Min(relativeAlses.Count, 6); size <= maxSize; size++)
					{
						foreach (var combination in relativeAlses.GetSubsets(size))
						{
							// Throw-when-use-out mode.
							var tempGrid = readOnlyGrid;
							tempGrid[cell] = digit;

							// When we set the value to true, all relative ALSes will be
							// degenerated to a subset.
							// Now remove the values as subset eliminations.
							foreach (var (_, _, digits, _, elimMap, _) in combination)
							{
								short subsetDigits = (short)(digits & ~(1 << digit));
								foreach (int subsetDigit in subsetDigits)
								{
									foreach (int elimCell in elimMap & CandMaps[subsetDigit])
									{
										// Remove the candidate.
										tempGrid[elimCell, subsetDigit] = false;
									}
								}
							}

							// Check whether there's any cell becoming a null cell.
							int nullCell = -1;
							for (int possibleNullCell = 0; possibleNullCell < 81; possibleNullCell++)
							{
								if (tempGrid.GetCandidates(possibleNullCell) == 0)
								{
									nullCell = possibleNullCell;
									break;
								}
							}

							if (nullCell == -1)
							{
								// Failed to search.
								continue;
							}

							// Death blossom found.
							// Now construct eliminations.
							var p = Cells.Empty;
							foreach (var als in combination)
							{
								p |= als.Map;
							}
							var realElimMap = (p & CandMaps[digit]).PeerIntersection & CandMaps[digit];
							var conclusions = new Conclusion[realElimMap.Count];
							int i = 0;
							foreach (int c in realElimMap)
							{
								conclusions[i++] = new(ConclusionType.Elimination, c, digit);
							}

							var views = new View[combination.Length + 1];
							var globalCells = new List<DrawingInfo>();
							for (int index = 1; index <= combination.Length; index++)
							{
								var alsCells = combination[index].Map;
								var cells = new List<DrawingInfo>();
								foreach (int c in alsCells)
								{
									var info = new DrawingInfo(index, c);
									cells.Add(info);
									globalCells.Add(info);
								}

								views[index] = new() { Cells = cells };
							}

							views[0] = new() { Cells = globalCells };

#if DEBUG
							accumulator.Add(
								new DbStepInfo(
									conclusions,
									views,
									cell,
									null!));
#endif
						}
					}
				}
			}
		}
	}
}
