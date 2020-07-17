using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.Singles;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.LinkType;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>Bowman's bingo</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.BowmanBingo))]
	[SearcherProperty(80, IsReadOnly = true)]
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


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var tempAccumulator = new List<BowmanBingoTechniqueInfo>();
			var tempGrid = grid.Clone();
			for (int digit = 0; digit < 9; digit++)
			{
				foreach (int cell in CandMaps[digit])
				{
					_tempConclusions.Add(new Conclusion(Assignment, cell, digit));
					var (candList, mask) = RecordUndoInfo(tempGrid, cell, digit);

					// Try to fill this cell.
					tempGrid[cell] = digit;
					int startCandidate = cell * 9 + digit;

					if (IsValidGrid(grid, cell))
					{
						GetAllRecursively(tempAccumulator, tempGrid, startCandidate, _length - 1);
					}
					else
					{
						tempAccumulator.Add(
							new BowmanBingoTechniqueInfo(
								conclusions: new[] { new Conclusion(Elimination, startCandidate) },
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets:
											new List<(int, int)>(
												from conclusion in _tempConclusions
												select (0, conclusion.CellOffset * 9 + conclusion.Digit)),
										regionOffsets: null,
										links: GetLinks())
								},
								contradictionSeries: new List<Conclusion>(_tempConclusions)));
					}

					// Undo the operation.
					_tempConclusions.RemoveLastElement();
					UndoGrid(tempGrid, candList, cell, mask);
				}
			}

			accumulator.AddRange(
				from info in tempAccumulator
				orderby info.ContradictionSeries.Count
				let Conclusion = info.ContradictionSeries[0]
				let Candidate = Conclusion.CellOffset * 9 + Conclusion.Digit
				orderby Candidate
				select info);
		}

		/// <summary>
		/// Take all information recursively.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="startCandidate">The start candidate.</param>
		/// <param name="length">The length.</param>
		private void GetAllRecursively(IList<BowmanBingoTechniqueInfo> result, Grid grid, int startCandidate, int length)
		{
			if (length == 0 || !(_searcher.GetOne(grid) is SingleTechniqueInfo singleInfo))
			{
				// Two cases we don't need to go on.
				// Case 1: the variable 'length' is 0.
				// Case 2: The searcher cannot get any new steps, which means the expression
				// always returns the value null. Therefore, this case (grid[cell] = digit)
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
				GetAllRecursively(result, grid, startCandidate, length - 1);
			}
			else
			{
				result.Add(
					new BowmanBingoTechniqueInfo(
						conclusions: new[] { new Conclusion(Elimination, startCandidate) },
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets:
									new List<(int, int)>(
										from tempConclusion in _tempConclusions
										select (0, tempConclusion.CellOffset * 9 + tempConclusion.Digit)),
								regionOffsets: null,
								links: GetLinks())
						},
						contradictionSeries: new List<Conclusion>(_tempConclusions)));
			}

			// Undo grid.
			_tempConclusions.RemoveLastElement();
			UndoGrid(grid, candList, c, mask);
		}

		/// <summary>
		/// Get links.
		/// </summary>
		/// <returns>The links.</returns>
		private IReadOnlyList<Link> GetLinks()
		{
			var result = new List<Link>();
			for (int i = 0, count = _tempConclusions.Count; i < count - 1; i++)
			{
				var (_, c1) = _tempConclusions[i];
				var (_, c2) = _tempConclusions[i + 1];
				result.Add(new Link(c1, c2, Default));
			}

			return result;
		}

		/// <summary>
		/// Record all information to be used in undo grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The result.</returns>
		private static (IReadOnlyList<int> _candList, short _mask) RecordUndoInfo(Grid grid, int cell, int digit)
		{
			var list = new List<int>();
			foreach (int c in PeerMaps[cell] & CandMaps[digit])
			{
				list.Add(c * 9 + digit);
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
		private static bool IsValidGrid(IReadOnlyGrid grid, int cell) =>
			Peers[cell].All(
				c =>
				{
					var status = grid.GetStatus(c);
					return (status != Empty && grid[c] != grid[cell] || status == Empty) && grid.GetCandidateMask(c) != 0;
				});
	}
}
