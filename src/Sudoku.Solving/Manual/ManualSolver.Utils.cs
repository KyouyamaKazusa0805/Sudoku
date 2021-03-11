using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Threading;
using Sudoku.Data;
using Sudoku.Models;
using Sudoku.Solving.Manual.Alses;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.Exocets;
using Sudoku.Solving.Manual.Fishes;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.RankTheory;
using Sudoku.Solving.Manual.Sdps;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Extended;
using Sudoku.Solving.Manual.Uniqueness.Loops;
using Sudoku.Solving.Manual.Uniqueness.Polygons;
using Sudoku.Solving.Manual.Uniqueness.Qiu;
using Sudoku.Solving.Manual.Uniqueness.Rects;
using Sudoku.Solving.Manual.Uniqueness.Reversal;
using Sudoku.Solving.Manual.Uniqueness.Square;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Gets all possible searchers via the current manual solver settings.
		/// </summary>
		/// <param name="solution">
		/// <para>(<see langword="in"/> parameter) The solution grid.</para>
		/// <para>
		/// This parameter is necessary because some technique searchers will use this value,
		/// such as <see cref="BfStepSearcher"/>. The default value is <see langword="null"/>.
		/// </para>
		/// </param>
		/// <returns>The result list.</returns>
		public StepSearcher[] GetSearchers(in SudokuGrid? solution)
		{
			var result = new List<StepSearcher>
			{
				new SingleStepSearcher
				{
					EnableFullHouse = EnableFullHouse,
					EnableLastDigit = EnableLastDigit,
					ShowDirectLines = ShowDirectLines
				},
				new LcStepSearcher(),
				new SubsetStepSearcher(),
				new NormalFishStepSearcher(),
				new RegularWingStepSearcher(CheckRegularWingSize),
				new IrregularWingStepSearcher(),
				new TwoStrongLinksStepSearcher(),
				new UrStepSearcher
				{
					AllowIncompleteUniqueRectangles = CheckIncompleteUniquenessPatterns,
					SearchForExtendedUniqueRectangles = SearchExtendedUniqueRectangles
				},
				new XrStepSearcher(),
				new UlStepSearcher(),
				new ErStepSearcher(),
				new AlcStepSearcher(CheckAlmostLockedQuadruple),
				new SdcStepSearcher(),
				new Sdc3dStepSearcher(),
				new BdpStepSearcher(),
				new ReverseBugStepSearcher(),
				new QdpStepSearcher(),
				new UsStepSearcher(),
				new GuardianStepSearcher(),
				new BugStepSearcher(UseExtendedBugSearcher),
				new EripStepSearcher(),
				new BivalueOddagonStepSearcher(),
				new AicStepSearcher(),
				new AlsXzStepSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new AlsXyWingStepSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new AlsWWingStepSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new DbStepSearcher(),
#if NISHIO_FORCING_CHAINS
				new FcStepSearcher(nishio: true, multiple: false, dynamic: true),
#else
				new ComplexFishStepSearcher { MaxSize = ComplexFishMaxSize },
#endif
				new FcStepSearcher(nishio: false, multiple: true, dynamic: false),
				new BugMultipleWithFcStepSearcher(),
				new FcStepSearcher(nishio: false, multiple: true, dynamic: true),
				new JeStepSearcher(CheckAdvancedInExocet),
				new SeStepSearcher(CheckAdvancedInExocet),
				new SkLoopStepSearcher(),
				new MslsStepSearcher(),
				new PomStepSearcher(),
				new BowmanBingoStepSearcher(BowmanBingoMaximumLength),
				new TemplateStepSearcher(OnlyRecordTemplateDelete),
#if DOUBLE_LAYERED_ASSUMPTION
				new FcPlusTechniqueSearcher(level: 1),
				new FcPlusTechniqueSearcher(level: 2),
				new FcPlusTechniqueSearcher(level: 3),
				new FcPlusTechniqueSearcher(level: 4),
				new FcPlusTechniqueSearcher(level: 5),
#endif
			};

			if (solution is { } s)
			{
				result.Add(new BfStepSearcher(s));
			}

			return result.ToArray();
		}

		/// <summary>
		/// Bound with on-solving methods returns the solving state.
		/// </summary>
		/// <param name="steps">The steps.</param>
		/// <param name="step">The step.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="cloneation">(<see langword="ref"/> parameter) The cloneation (playground).</param>
		/// <param name="stopwatch">The stopwatch.</param>
		/// <param name="stepGrids">The step grids.</param>
		/// <param name="result">(<see langword="out"/> parameter) The analysis result.</param>
		/// <param name="cancellationToken">The cancellation token that is used to cancel the operation.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <exception cref="OperationCanceledException">
		/// Throws when the current operation is cancelled.
		/// </exception>
		private bool RecordStep(
			IList<StepInfo> steps, StepInfo step, in SudokuGrid grid,
			ref SudokuGrid cloneation, Stopwatch stopwatch, IList<SudokuGrid> stepGrids,
			[NotNullWhen(true)] out AnalysisResult? result, CancellationToken? cancellationToken)
		{
			bool needAdd = false;
			foreach (var (t, c, d) in step.Conclusions)
			{
				switch (t)
				{
					case ConclusionType.Assignment when cloneation.GetStatus(c) == CellStatus.Empty:
					case ConclusionType.Elimination when cloneation.Exists(c, d) is true:
					{
						needAdd = true;

						goto FinalCheck;
					}
				}
			}

		FinalCheck:
			if (needAdd)
			{
				stepGrids.Add(cloneation);
				step.ApplyTo(ref cloneation);
				steps.Add(step);

				if (cloneation.IsSolved)
				{
					result = new(SolverName, grid, true, stopwatch.Elapsed)
					{
						Steps = steps.AsReadOnlyList(),
						StepGrids = stepGrids.AsReadOnlyList(),
					};
					return true;
				}
			}

			cancellationToken?.ThrowIfCancellationRequested();

			result = null;
			return false;
		}


		/// <summary>
		/// To check the validity of all conclusions.
		/// </summary>
		/// <param name="solution">(<see langword="in"/> parameter) The solution.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckConclusionsValidity(in SudokuGrid solution, IEnumerable<Conclusion> conclusions)
		{
			foreach (var (t, c, d) in conclusions)
			{
				int digit = solution[c];
				switch (t)
				{
					case ConclusionType.Assignment when digit != d:
					case ConclusionType.Elimination when digit == d:
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// To report the progress.
		/// </summary>
		/// <param name="cloneation">(<see langword="in"/> parameter) The cloneation grid.</param>
		/// <param name="progress">The progress reporter.</param>
		/// <param name="progressResult">(<see langword="ref"/> parameter) The progress result.</param>
		private static void ReportProgress(
			in SudokuGrid cloneation, IProgress<IProgressResult> progress, ref GridProgressResult progressResult)
		{
			progressResult.CurrentCandidatesCount = cloneation.CandidatesCount;
			progressResult.CurrentCellsCount = cloneation.EmptiesCount;
			progress.Report(progressResult);
		}
	}
}
