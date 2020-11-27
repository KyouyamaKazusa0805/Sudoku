using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Models;
using Sudoku.Solving.Manual.Alses;
using Sudoku.Solving.Manual.Alses.Basic;
using Sudoku.Solving.Manual.Alses.Mslses;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.Exocets;
using Sudoku.Solving.Manual.Fishes;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.Sdps;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Extended;
using Sudoku.Solving.Manual.Uniqueness.Loops;
using Sudoku.Solving.Manual.Uniqueness.Polygons;
using Sudoku.Solving.Manual.Uniqueness.Qiu;
using Sudoku.Solving.Manual.Uniqueness.Rects;
using Sudoku.Solving.Manual.Uniqueness.Square;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Get all searchers using in Hodoku-like mode.
		/// </summary>
		/// <param name="solution">(<see langword="in"/> parameter) The solution used for brute forces.</param>
		/// <returns>The searchers.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TechniqueSearcher[] GetSearchersHodokuMode(in SudokuGrid solution) =>
			new TechniqueSearcher[]
			{
				new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit, ShowDirectLines),
				new LcTechniqueSearcher(),
				new SubsetTechniqueSearcher(),
				new NormalFishTechniqueSearcher(),
				new RegularWingTechniqueSearcher(CheckRegularWingSize),
				new IrregularWingTechniqueSearcher(),
				new TwoStrongLinksTechniqueSearcher(),
				new UrTechniqueSearcher(CheckIncompleteUniquenessPatterns, SearchExtendedUniqueRectangles),
				new XrTechniqueSearcher(),
				new UlTechniqueSearcher(),
				new EmptyRectangleTechniqueSearcher(),
				new AlcTechniqueSearcher(CheckAlmostLockedQuadruple),
				new SdcTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new BdpTechniqueSearcher(),
				new QdpTechniqueSearcher(),
				new UsTechniqueSearcher(),
				new GuardianTechniqueSearcher(),
				new BugTechniqueSearcher(UseExtendedBugSearcher),
				new EripTechniqueSearcher(),
				new AicTechniqueSearcher(),
				new AlsXzTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new AlsXyWingTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new AlsWWingTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				//new DeathBlossomTechniqueSearcher(
				//	AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, MaxPetalsOfDeathBlossom),
				new DbTechniqueSearcher(MaxPetalsOfDeathBlossom),
				new FcTechniqueSearcher(true, false, true, 0),
				new FcTechniqueSearcher(false, true, false, 0),
				new FcTechniqueSearcher(false, true, true, 0),
				new BugMultipleWithFcTechniqueSearcher(),
				new HobiwanFishTechniqueSearcher(
					HobiwanFishMaximumSize, HobiwanFishMaximumExofinsCount,
					HobiwanFishMaximumEndofinsCount, HobiwanFishCheckTemplates),
				new JuniorExocetTechniqueSearcher(CheckAdvancedInExocet),
				new SeniorExocetTechniqueSearcher(CheckAdvancedInExocet),
				new SkLoopTechniqueSearcher(),
				new AlsNetTechniqueSearcher(),
				new PomTechniqueSearcher(),
				new BowmanBingoTechniqueSearcher(BowmanBingoMaximumLength),
				new TemplateTechniqueSearcher(OnlyRecordTemplateDelete),
				new BruteForceTechniqueSearcher(solution),
			};

#if OBSOLETE || true
		/// <summary>
		/// Get all searchers using in Sudoku Explainer-like mode.
		/// </summary>
		/// <param name="solution">(<see langword="in"/> parameter) The solution used for brute forces.</param>
		/// <returns>The searchers.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TechniqueSearcher[][] GetSearchersSeMode(in SudokuGrid solution) =>
			new TechniqueSearcher[][]
			{
				new[] { new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit, ShowDirectLines) },
				new[] { new LcTechniqueSearcher() },
				new TechniqueSearcher[]
				{
					new SubsetTechniqueSearcher(),
					new NormalFishTechniqueSearcher(),
					new RegularWingTechniqueSearcher(CheckRegularWingSize),
					new IrregularWingTechniqueSearcher(),
					new TwoStrongLinksTechniqueSearcher(),
					new UrTechniqueSearcher(CheckIncompleteUniquenessPatterns, SearchExtendedUniqueRectangles),
					new XrTechniqueSearcher(),
					new UlTechniqueSearcher(),
					new EmptyRectangleTechniqueSearcher(),
					new AlcTechniqueSearcher(CheckAlmostLockedQuadruple),
					new SdcTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
					new BdpTechniqueSearcher(),
					new QdpTechniqueSearcher(),
					new UsTechniqueSearcher(),
					new GuardianTechniqueSearcher(),
					new BugTechniqueSearcher(UseExtendedBugSearcher),
					new EripTechniqueSearcher(),
					new AicTechniqueSearcher(),
					new AlsXzTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
					new AlsXyWingTechniqueSearcher(
						AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
					new AlsWWingTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				},
				new TechniqueSearcher[]
				{
					new FcTechniqueSearcher(true, false, true, 0),
					new FcTechniqueSearcher(false, true, false, 0),
					new FcTechniqueSearcher(false, true, true, 0),
					new BugMultipleWithFcTechniqueSearcher(),
					new BowmanBingoTechniqueSearcher(BowmanBingoMaximumLength),
					//new DeathBlossomTechniqueSearcher(
					//	AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, MaxPetalsOfDeathBlossom),
					new DbTechniqueSearcher(MaxPetalsOfDeathBlossom),
					new HobiwanFishTechniqueSearcher(
						HobiwanFishMaximumSize, HobiwanFishMaximumExofinsCount,
						HobiwanFishMaximumEndofinsCount, HobiwanFishCheckTemplates),
					new PomTechniqueSearcher(),
					new TemplateTechniqueSearcher(OnlyRecordTemplateDelete),
				},
				new TechniqueSearcher[]
				{
					new JuniorExocetTechniqueSearcher(CheckAdvancedInExocet),
					new SeniorExocetTechniqueSearcher(CheckAdvancedInExocet),
					new SkLoopTechniqueSearcher(),
					new AlsNetTechniqueSearcher(),
				},
				new[] { new BruteForceTechniqueSearcher(solution) }
			};
#endif

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
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool RecordTechnique(
			List<TechniqueInfo> steps, TechniqueInfo step, in SudokuGrid grid,
			ref SudokuGrid cloneation, Stopwatch stopwatch, List<SudokuGrid> stepGrids,
			[NotNullWhen(true)] out AnalysisResult? result)
		{
			bool needAdd = false;
			foreach (var (t, c, d) in step.Conclusions)
			{
				switch (t)
				{
					case Assignment when cloneation.GetStatus(c) == CellStatus.Empty:
					case Elimination when cloneation.Exists(c, d) is true:
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

				if (cloneation.HasSolved)
				{
					result = new(
						solverName: SolverName,
						puzzle: grid,
						solution: cloneation,
						hasSolved: true,
						elapsedTime: stopwatch.Elapsed,
						steps,
						stepGrids);
					return true;
				}
			}

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
					case Assignment when digit != d:
					case Elimination when digit == d:
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

		/// <summary>
		/// The internal selector.
		/// </summary>
		/// <param name="info">The information.</param>
		/// <returns>The decimal selection.</returns>
		private static decimal InternalSelector(TechniqueInfo info) => info.Difficulty;

		/// <summary>
		/// Internal checking.
		/// </summary>
		/// <param name="info">The technique information.</param>
		/// <param name="solution">(<see langword="in"/> parameter) The solution.</param>
		/// <returns>The result.</returns>
		private static bool InternalChecking(TechniqueInfo info, in SudokuGrid solution) =>
			CheckConclusionsValidity(solution, info.Conclusions);
	}
}
