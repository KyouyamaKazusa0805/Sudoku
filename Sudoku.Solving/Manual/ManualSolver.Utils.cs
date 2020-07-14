using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.ComponentModel;
using Sudoku.Data;
using Sudoku.Data.Extensions;
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
		/// <param name="solution">The solution used for brute forces.</param>
		/// <returns>The searchers.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TechniqueSearcher[] GetSearchersHodokuMode(IReadOnlyGrid solution) =>
			new TechniqueSearcher[]
			{
				new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit),
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
				new ErIntersectionPairTechniqueSearcher(),
				new AicTechniqueSearcher(),
				new AlsXzTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new AlsXyWingTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new AlsWWingTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new DeathBlossomTechniqueSearcher(
					AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, MaxPetalsOfDeathBlossom),
				new FcTechniqueSearcher(),
				new HobiwanFishTechniqueSearcher(
					HobiwanFishMaximumSize, HobiwanFishMaximumExofinsCount,
					HobiwanFishMaximumEndofinsCount, HobiwanFishCheckTemplates),
				new JuniorExocetTechniqueSearcher(CheckAdvancedInExocet),
				new SeniorExocetTechniqueSearcher(CheckAdvancedInExocet),
				new SkLoopTechniqueSearcher(),
				new PomTechniqueSearcher(),
				new BowmanBingoTechniqueSearcher(BowmanBingoMaximumLength),
				new TemplateTechniqueSearcher(OnlyRecordTemplateDelete),
				new AlsNetTechniqueSearcher(),
				new BruteForceTechniqueSearcher(solution),
			};

		/// <summary>
		/// Get all searchers using in Sudoku Explainer-like mode.
		/// </summary>
		/// <param name="solution">The solution used for brute forces.</param>
		/// <returns>The searchers.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TechniqueSearcher[][] GetSearchersSeMode(IReadOnlyGrid solution) =>
			new TechniqueSearcher[][]
			{
				new[] { new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit) },
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
					new ErIntersectionPairTechniqueSearcher(),
					new AicTechniqueSearcher(),
					new AlsXzTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
					new AlsXyWingTechniqueSearcher(
						AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
					new AlsWWingTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				},
				new TechniqueSearcher[]
				{
					new FcTechniqueSearcher(),
					new BowmanBingoTechniqueSearcher(BowmanBingoMaximumLength),
					new DeathBlossomTechniqueSearcher(
						AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, MaxPetalsOfDeathBlossom),
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

		/// <summary>
		/// Bound with on-solving methods returns the solving state.
		/// </summary>
		/// <param name="steps">The steps.</param>
		/// <param name="step">The step.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation (playground).</param>
		/// <param name="stopwatch">The stopwatch.</param>
		/// <param name="stepGrids">The step grids.</param>
		/// <param name="result">(<see langword="out"/> parameter) The analysis result.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool RecordTechnique(
			List<TechniqueInfo> steps, TechniqueInfo step, IReadOnlyGrid grid,
			Grid cloneation, Stopwatch stopwatch, List<IReadOnlyGrid> stepGrids,
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

						goto Label_Determine;
					}
				}
			}

		Label_Determine:
			if (needAdd)
			{
				stepGrids.Add(cloneation.Clone());
				step.ApplyTo(cloneation);
				steps.Add(step);

				if (cloneation.HasSolved)
				{
					result = new AnalysisResult(
						solverName: SolverName,
						puzzle: grid,
						solution: cloneation,
						hasSolved: true,
						elapsedTime: stopwatch.Elapsed,
						steps,
						stepGrids,
						additional: null);
					return true;
				}
			}

			result = null;
			return false;
		}


		/// <summary>
		/// To check the validity of all conclusions.
		/// </summary>
		/// <param name="solution">The solution.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckConclusionsValidity(IReadOnlyGrid solution, IEnumerable<Conclusion> conclusions)
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
		/// <param name="cloneation">The cloneation grid.</param>
		/// <param name="progress">The progress reporter.</param>
		/// <param name="progressResult">(<see langword="ref"/> parameter) The progress result.</param>
		private static void ReportProgress(
			IReadOnlyGrid cloneation, IProgress<IProgressResult> progress,
			ref GridProgressResult progressResult)
		{
			progressResult.CurrentCandidatesCount = cloneation.CandidatesCount;
			progressResult.CurrentCellsCount = cloneation.EmptiesCount;
			progress.Report(progressResult);
		}
	}
}
