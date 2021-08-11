using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Techniques;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>Bowman's bingo</b> technique searcher.
	/// </summary>
	/// <remarks>
	/// This searcher only uses hidden singles and naked singles as the iteration condition. Therefore,
	/// "Dynamic" Bowman's Bingo can't be found here.
	/// </remarks>
	[IsOptionsFixed]
	public sealed class BowmanBingoStepSearcher : LastResortStepSearcher
	{
		/// <summary>
		/// The singles searcher.
		/// </summary>
		private readonly SingleStepSearcher _searcher = new() { EnableFullHouse = true, EnableLastDigit = true };

		/// <summary>
		/// All temporary conclusions.
		/// </summary>
		private readonly IList<Conclusion> _tempConclusions = new List<Conclusion>();


		/// <summary>
		/// Indicates the maximum length of the bowman bingo you want to search for.
		/// </summary>
		public int MaxLength { get; set; }

		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(
			19, DisplayingLevel.None,
			EnabledAreas: EnabledAreas.None,
			DisabledReason: DisabledReason.LastResort
		);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(19, nameof(Technique.BowmanBingo))
		{
			DisplayLevel = 3,
			IsReadOnly = true,
			IsEnabled = false,
			DisabledReason = DisabledReason.LastResort
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var tempAccumulator = new List<BowmanBingoStepInfo>();
			var tempGrid = grid;
			for (int digit = 0; digit < 9; digit++)
			{
				foreach (int cell in CandMaps[digit])
				{
					_tempConclusions.Add(new(ConclusionType.Assignment, cell, digit));
					var (candList, mask) = RecordUndoInfo(tempGrid, cell, digit);

					// Try to fill this cell.
					tempGrid[cell] = digit;
					int startCandidate = cell * 9 + digit;

					if (IsValidGrid(grid, cell))
					{
						GetAll(tempAccumulator, ref tempGrid, startCandidate, MaxLength - 1);
					}
					else
					{
						var candidateOffsets = new DrawingInfo[_tempConclusions.Count];
						int i = 0;
						foreach (var (_, candidate) in _tempConclusions)
						{
							candidateOffsets[i++] = new(0, candidate);
						}

						tempAccumulator.Add(
							new BowmanBingoStepInfo(
								new Conclusion[] { new(ConclusionType.Elimination, startCandidate) },
								new View[] { new() { Candidates = candidateOffsets, Links = GetLinks() } },
								_tempConclusions.AsReadOnlyList()
							)
						);
					}

					// Undo the operation.
					_tempConclusions.RemoveLastElement();
					UndoGrid(ref tempGrid, candList, cell, mask);
				}
			}

			accumulator.AddRange(
				from info in tempAccumulator
				orderby info.ContradictionSeries.Count, info.ContradictionSeries[0]
				select info
			);
		}

		/// <summary>
		/// Take all information recursively.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="startCand">The start candidate.</param>
		/// <param name="length">The length.</param>
		private void GetAll(IList<BowmanBingoStepInfo> result, ref SudokuGrid grid, int startCand, int length)
		{
			if (length == 0 || _searcher.GetOne(grid) is not SingleStepInfo singleInfo)
			{
				// Two cases we don't need to go on.
				// Case 1: the variable 'length' is 0.
				// Case 2: The searcher can't get any new steps, which means the expression
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
				GetAll(result, ref grid, startCand, length - 1);
			}
			else
			{
				var candidateOffsets = new DrawingInfo[_tempConclusions.Count];
				int i = 0;
				foreach (var (_, candidate) in _tempConclusions)
				{
					candidateOffsets[i++] = new(0, candidate);
				}

				result.Add(
					new BowmanBingoStepInfo(
						new Conclusion[] { new(ConclusionType.Elimination, startCand) },
						new View[] { new() { Candidates = candidateOffsets, Links = GetLinks() } },
						_tempConclusions.AsReadOnlyList()
					)
				);
			}

			// Undo grid.
			_tempConclusions.RemoveLastElement();
			UndoGrid(ref grid, candList, c, mask);
		}

		/// <summary>
		/// Get links.
		/// </summary>
		/// <returns>The links.</returns>
		private IReadOnlyList<Link> GetLinks()
		{
			var result = new List<Link>();
			for (int i = 0, iterationCount = _tempConclusions.Count - 1; i < iterationCount; i++)
			{
				var (_, c1) = _tempConclusions[i];
				var (_, c2) = _tempConclusions[i + 1];
				result.Add(new(c1, c2, LinkType.Default));
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
		private static (IReadOnlyList<int> CandidateList, short Mask) RecordUndoInfo(
			in SudokuGrid grid, int cell, int digit)
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
		private static void UndoGrid(ref SudokuGrid grid, IReadOnlyList<int> list, int cell, short mask)
		{
			foreach (int cand in list)
			{
				grid[cand / 9, cand % 9] = true;
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
		private static bool IsValidGrid(in SudokuGrid grid, int cell)
		{
			bool result = true;
			foreach (int peerCell in Peers[cell])
			{
				var status = grid.GetStatus(peerCell);
				if (!(status != CellStatus.Empty && grid[peerCell] != grid[cell] || status == CellStatus.Empty)
					|| grid.GetCandidates(peerCell) == 0)
				{
					result = false;
					break;
				}
			}

			return result;
		}
	}
}
