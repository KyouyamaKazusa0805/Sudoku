#define COMPLEX_FISH

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
		/// <para>The solution grid.</para>
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
				new RegularWingStepSearcher { MaxSize = CheckRegularWingSize },
				new IrregularWingStepSearcher(),
				new TwoStrongLinksStepSearcher(),
				new UrStepSearcher
				{
					AllowIncompleteUniqueRectangles = CheckIncompleteUniquenessPatterns,
					SearchForExtendedUniqueRectangles = SearchExtendedUniqueRectangles
				},
				new ComplexFishStepSearcher { MaxSize = 2 },
				new XrStepSearcher(),
				new UlStepSearcher(),
				new ErStepSearcher(),
				new AlcStepSearcher { CheckAlmostLockedQuadruple = CheckAlmostLockedQuadruple },
				new SdcStepSearcher(),
				new Sdc3dStepSearcher(),
				new BdpStepSearcher(),
				new ReverseBugStepSearcher(),
				new QdpStepSearcher(),
				new UsStepSearcher(),
				new GuardianStepSearcher(),
				new BugStepSearcher { SearchExtendedBugTypes = UseExtendedBugSearcher },
				new EripStepSearcher(),
				new BivalueOddagonStepSearcher(),
				new AicStepSearcher(),
				new AlsXzStepSearcher
				{
					AllowOverlapping = AllowOverlappingAlses,
					AlsShowRegions = AlsHighlightRegionInsteadOfCell,
					AllowAlsCycles = AllowAlsCycles
				},
				new AlsXyWingStepSearcher
				{
					AllowOverlapping = AllowOverlappingAlses,
					AlsShowRegions = AlsHighlightRegionInsteadOfCell,
					AllowAlsCycles = AllowAlsCycles
				},
				new AlsWWingStepSearcher
				{
					AllowOverlapping = AllowOverlappingAlses,
					AlsShowRegions = AlsHighlightRegionInsteadOfCell,
					AllowAlsCycles = AllowAlsCycles
				},
				new DbStepSearcher(),
#if COMPLEX_FISH
				new ComplexFishStepSearcher { MaxSize = ComplexFishMaxSize },
#elif NISHIO_FORCING_CHAINS
				new FcStepSearcher(nishio: true, multiple: false, dynamic: true),
#else
#warning If both 'COMPLEX_FISH' and 'NISHIO_FORCING_CHAINS' aren't defined, the fish-like techniques won't be found.
#endif
				new FcStepSearcher(nishio: false, multiple: true, dynamic: false),
				new BugMultipleWithFcStepSearcher(),
				new FcStepSearcher(nishio: false, multiple: true, dynamic: true),
				new JeStepSearcher { CheckAdvanced = CheckAdvancedInExocet },
				new SeStepSearcher { CheckAdvanced = CheckAdvancedInExocet },
				new SkLoopStepSearcher(),
				new MslsStepSearcher(),
				new PomStepSearcher(),
				new BowmanBingoStepSearcher { MaxLength = BowmanBingoMaximumLength },
			};

			if (solution is { } s)
			{
				result.Add(new TemplateStepSearcher(s) { TemplateDeleteOnly = OnlyRecordTemplateDelete });
				result.Add(new BfStepSearcher(s));
			}

			return result.ToArray();
		}

		/// <summary>
		/// Bound with on-solving methods returns the solving state.
		/// </summary>
		/// <param name="steps">The steps.</param>
		/// <param name="step">The step.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation (playground).</param>
		/// <param name="stopwatch">The stopwatch.</param>
		/// <param name="stepGrids">The step grids.</param>
		/// <param name="result">The analysis result.</param>
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
		/// Get all available SSTS searchers.
		/// </summary>
		/// <param name="enableFullHouse">
		/// Set an option to <see cref="SingleStepSearcher.EnableFullHouse"/>.
		/// </param>
		/// <param name="enableLastDigit">
		/// Set an option to <see cref="SingleStepSearcher.EnableLastDigit"/>.
		/// </param>
		/// <param name="showDirectLines">
		/// Set an option to <see cref="SingleStepSearcher.ShowDirectLines"/>.
		/// </param>
		/// <returns>Returns the list of SSTS searchers.</returns>
		/// <seealso cref="SingleStepSearcher.EnableFullHouse"/>
		/// <seealso cref="SingleStepSearcher.EnableLastDigit"/>
		/// <seealso cref="SingleStepSearcher.ShowDirectLines"/>
		public static StepSearcher[] GetSstsSearchers(
			bool enableFullHouse = false, bool enableLastDigit = false, bool showDirectLines = false) =>
			new StepSearcher[]
			{
				new SingleStepSearcher
				{
					EnableFullHouse = enableFullHouse,
					EnableLastDigit = enableLastDigit,
					ShowDirectLines = showDirectLines
				},
				new LcStepSearcher(),
				new SubsetStepSearcher()
			};

		/// <summary>
		/// To check the validity of all conclusions.
		/// </summary>
		/// <param name="solution">The solution.</param>
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
		/// <param name="cloneation">The cloneation grid.</param>
		/// <param name="progress">The progress reporter.</param>
		/// <param name="progressResult">The progress result.</param>
		private static void ReportProgress(
			in SudokuGrid cloneation, IProgress<IProgressResult> progress, ref GridProgressResult progressResult)
		{
			progressResult.CurrentCandidatesCount = cloneation.CandidatesCount;
			progressResult.CurrentCellsCount = cloneation.EmptiesCount;
			progress.Report(progressResult);
		}
	}
}
