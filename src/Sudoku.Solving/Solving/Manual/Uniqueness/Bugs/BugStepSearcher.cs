using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Solving.Checking;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Encapsulates a <b>bivalue universal grave</b> (BUG) technique searcher.
	/// </summary>
	public sealed partial class BugStepSearcher : UniquenessStepSearcher
	{
		/// <summary>
		/// Indicates whether the searcher should call the extended BUG checker
		/// to search for all true candidates no matter how difficult searching.
		/// </summary>
		public bool SearchExtendedBugTypes { get; init; }


		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		public static TechniqueProperties Properties { get; } = new(25, nameof(Technique.BugType1))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var trueCandidates = new BugChecker(grid).TrueCandidates;
			switch (trueCandidates.Count)
			{
				case 0:
				{
					return;
				}
				case 1:
				{
					// BUG + 1 found.
					accumulator.Add(
						new BugType1StepInfo(
							new Conclusion[] { new(ConclusionType.Assignment, trueCandidates[0]) },
							new View[] { new() { Candidates = new DrawingInfo[] { new(0, trueCandidates[0]) } } }
						)
					);
					break;
				}
				default:
				{
					if (CheckSingleDigit(trueCandidates))
					{
						CheckType2(accumulator, trueCandidates);
					}
					else
					{
						if (SearchExtendedBugTypes)
						{
							CheckMultiple(accumulator, grid, trueCandidates);
							CheckXz(accumulator, grid, trueCandidates);
						}

						CheckType3Naked(accumulator, grid, trueCandidates);
						CheckType4(accumulator, grid, trueCandidates);
					}

					break;
				}
			}
		}

		partial void CheckType2(IList<StepInfo> accumulator, IReadOnlyList<int> trueCandidates);
		partial void CheckType3Naked(IList<StepInfo> accumulator, in SudokuGrid grid, IReadOnlyList<int> trueCandidates);
		partial void CheckType4(IList<StepInfo> accumulator, in SudokuGrid grid, IReadOnlyList<int> trueCandidates);
		partial void CheckMultiple(IList<StepInfo> accumulator, in SudokuGrid grid, IReadOnlyList<int> trueCandidates);
		partial void CheckXz(IList<StepInfo> accumulator, in SudokuGrid grid, IReadOnlyList<int> trueCandidates);
	}
}
