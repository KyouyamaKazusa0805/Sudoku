using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>Bowman's bingo</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Bowman's Bingo")]
	public sealed class BowmanBingoTechniqueSearcher : LastResortTechniqueSearcher
	{
		/// <summary>
		/// Indicates the length to find.
		/// </summary>
		private readonly int _length;

		/// <summary>
		/// The singles searcher.
		/// </summary>
		private readonly SingleTechniqueSearcher _searcher = new SingleTechniqueSearcher(true, true);

		/// <summary>
		/// All temporary conclusions.
		/// </summary>
		private readonly IList<Conclusion> _tempConclusions = new List<Conclusion>();


		/// <summary>
		/// Initializes an instance with the specified length.
		/// </summary>
		/// <param name="length">The length.</param>
		public BowmanBingoTechniqueSearcher(int length) => _length = length;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 80;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var tempGrid = grid.Clone();
			for (int cell = 0; cell < 81; cell++)
			{
				if (tempGrid.GetCellStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				for (int digit = 0; digit < 9; digit++)
				{
					if (tempGrid.CandidateDoesNotExist(cell, digit))
					{
						continue;
					}

					_tempConclusions.Add(new Conclusion(ConclusionType.Assignment, cell, digit));
					var (candList, mask) = RecordUndoInfo(tempGrid, cell, digit);

					// Try to fill this cell.
					tempGrid[cell] = digit;
					int startCandidate = cell * 9 + digit;

					if (IsValidGrid(grid, cell))
					{
						TakeAllRecursively(accumulator, tempGrid, startCandidate, _length - 1);
					}
					else
					{
						accumulator.Add(
							new BowmanBingoTechniqueInfo(
								conclusions: new[]
								{
									new Conclusion(ConclusionType.Elimination, startCandidate)
								},
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets:
											new List<(int, int)>(
												from conclusion in _tempConclusions
												select (0, conclusion.CellOffset * 9 + conclusion.Digit)),
										regionOffsets: null,
										links: null)
								},
								contradictionSeries: new List<Conclusion>(_tempConclusions)));
					}

					// Undo the operation.
					_tempConclusions.RemoveLastElement();
					UndoGrid(tempGrid, candList, cell, mask);
				}
			}
		}

		private void TakeAllRecursively(
			IBag<TechniqueInfo> result, Grid grid, int startCandidate, int length)
		{
			if (length == 0)
			{
				return;
			}

			var info = _searcher.TakeOne(grid);
			if (!(info is SingleTechniqueInfo singleInfo))
			{
				// The searcher cannot find any steps next.
				// which means that this case (grid[cell] = digit)
				// is a bad try.
				return;
			}

			// Try to fill.
			var conclusion = singleInfo.Conclusions[0];
			_tempConclusions.Add(conclusion);
			var (_, c, d) = conclusion;
			var (candList, mask) = RecordUndoInfo(grid, c, d);

			grid[c] = d;
			if (IsValidGrid(grid, c))
			{
				// Sounds good.
				TakeAllRecursively(result, grid, startCandidate, length - 1);
			}
			else
			{
				result.Add(
					new BowmanBingoTechniqueInfo(
						conclusions: new[]
						{
							new Conclusion(ConclusionType.Elimination, startCandidate)
						},
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets:
									new List<(int, int)>(
										from tempConclusion in _tempConclusions
										select (0, tempConclusion.CellOffset * 9 + tempConclusion.Digit)),
								regionOffsets: null,
								links: null)
						},
						contradictionSeries: new List<Conclusion>(_tempConclusions)));
			}

			// Undo grid.
			_tempConclusions.RemoveLastElement();
			UndoGrid(grid, candList, c, mask);
		}

		/// <summary>
		/// Record all information to be used in undo grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The result.</returns>
		private static (IReadOnlyList<int> _candList, short _mask) RecordUndoInfo(
			Grid grid, int cell, int digit)
		{
			var list = new List<int>();
			foreach (int c in new GridMap(cell, false).Offsets)
			{
				if (grid.CandidateExists(c, digit))
				{
					list.Add(c * 9 + digit);
				}
			}

			return (list, grid.GetMask(cell));
		}

		/// <summary>
		/// Undo the grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="list">The list.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="mask">The mask.</param>
		private static void UndoGrid(Grid grid, IReadOnlyList<int> list, int cell, short mask)
		{
			foreach (int cand in list)
			{
				grid[cand / 9, cand % 9] = false;
			}

			grid.SetMask(cell, mask);
		}

		/// <summary>
		/// To check the specified cell has a same digit filled in a cell
		/// which is same region with the current one.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <returns>The result.</returns>
		private static bool IsValidGrid(IReadOnlyGrid grid, int cell)
		{
			return new GridMap(cell, false).Offsets.All(c =>
			{
				var status = grid.GetCellStatus(c);
				return (
					status != CellStatus.Empty && grid[c] != grid[cell]
					|| status == CellStatus.Empty
				) && grid.GetCandidates(c) != 511;
			});
		}
	}
}
