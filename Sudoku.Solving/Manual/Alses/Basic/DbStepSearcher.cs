using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
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
			var readonlyGrid = grid;

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
						bool condition(int cell) => readonlyGrid.Exists(cell, digit) is not true;

						// If the peer intersection contains that cell, the ALS is relative one.
						// Add into the list.
						if (appearing.PeerIntersection.Contains(cell))
						{
							relativeAlses.Add(als);
						}
					}

					for (int size = 2, maxSize = relativeAlses.Count; size <= maxSize; size++)
					{
						foreach (var combination in relativeAlses.GetSubsets(size))
						{
							// Throw-when-use-out mode.
							var tempGrid = grid;
							tempGrid[cell] = digit;

							// TODO: Wait for implementation.
						}
					}
				}
			}
		}
	}
}
