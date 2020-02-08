using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Runtime;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Utils;
using BugType1 = Sudoku.Solving.Manual.Uniqueness.Bugs.BivalueUniversalGraveTechniqueInfo;
using BugType2 = Sudoku.Solving.Manual.Uniqueness.Bugs.BivalueUniversalGraveType2TechniqueInfo;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Encapsulates a bivalue universal grave (BUG) technique searcher.
	/// </summary>
	public sealed class BivalueUniversalGraveTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <inheritdoc/>
		/// <exception cref="WrongHandlingException">
		/// Throws when the number of true candidates is naught.
		/// </exception>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var checker = new BugChecker(grid);
			var trueCandidates = checker.TrueCandidates;
			if (trueCandidates.Count == 0)
			{
				return Array.Empty<UniquenessTechniqueInfo>();
			}

			var result = new List<UniquenessTechniqueInfo>();
			int trueCandidatesCount = trueCandidates.Count;
			switch (trueCandidatesCount)
			{
				case 0:
				{
					throw new WrongHandlingException(grid);
				}
				case 1:
				{
					// BUG + 1 found.
					result.Add(
						new BugType1(
							conclusions: new[] { new Conclusion(ConclusionType.Assignment, trueCandidates[0]) },
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets: new[] { (0, trueCandidates[0]) },
									regionOffsets: null,
									linkMasks: null)
							}));
					break;
				}
				default:
				{
					if (CheckSingleDigit(trueCandidates))
					{
						CheckType2(grid, trueCandidates, result);
					}
					else
					{

					}

					break;
				}
			}

			return result;
		}

		#region BUG utils
		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		/// <param name="result">The result list.</param>
		private static void CheckType2(
			Grid grid, IReadOnlyList<int> trueCandidates, IList<UniquenessTechniqueInfo> result)
		{
			var map = default(GridMap);
			int i = 0;
			foreach (int cand in trueCandidates)
			{
				if (i++ == 0)
				{
					map = new GridMap(cand / 9);
				}
				else
				{
					map &= new GridMap(cand / 9);
				}
			}

			if (map.Count != 0)
			{
				// BUG type 2 found.
				// Check eliminations.
				var conclusions = new List<Conclusion>();
				int digit = trueCandidates[0] % 9;
				foreach (int cell in map.Offsets)
				{
					if (grid.CandidateExists(cell, digit))
					{
						conclusions.Add(
							new Conclusion(
								ConclusionType.Elimination, cell * 9 + digit));
					}
				}

				if (conclusions.Count != 0)
				{
					// BUG type 2.
					result.Add(
						new BugType2(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets:
										new List<(int, int)>(
											from cand in trueCandidates select (0, cand)),
									regionOffsets: null,
									linkMasks: null)
							},
							digit,
							cells: trueCandidates));
				}
			}
		}

		/// <summary>
		/// Check whether all candidates in the list has same digit value.
		/// </summary>
		/// <param name="list">The list of all true candidates.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckSingleDigit(IReadOnlyList<int> list)
		{
			int i = 0;
			int comparer = default;
			foreach (int cand in list)
			{
				if (i++ == 0)
				{
					comparer = cand % 9;
					continue;
				}

				if (comparer != cand % 9)
				{
					return false;
				}
			}

			return true;
		}
		#endregion
	}
}
