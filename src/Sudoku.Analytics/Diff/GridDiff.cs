using System.Diagnostics.CodeAnalysis;
using Sudoku.Analytics;
using Sudoku.Analytics.StepSearchers;
using Sudoku.Concepts;
using Sudoku.Linq;
using static Sudoku.Analytics.ConclusionType;

namespace Sudoku.Diff;

/// <summary>
/// Provides the method to operate with <see cref="Grid"/> instances, checking for the diff of two <see cref="Grid"/> instances.
/// </summary>
public static class GridDiff
{
	/// <summary>
	/// The internal step collector.
	/// </summary>
	private static readonly StepCollector StepCollector = new StepCollector()
		.WithSameLevelConfigruation(StepCollectorDifficultyLevelMode.All)
		.WithStepSearcherSetters<SingleStepSearcher>(static s => { s.EnableFullHouse = true; s.EnableLastDigit = true; s.HiddenSinglesInBlockFirst = true; s.UseIttoryuMode = false; })
		.WithStepSearcherSetters<UniqueRectangleStepSearcher>(static s => { s.AllowIncompleteUniqueRectangles = true; s.SearchForExtendedUniqueRectangles = true; })
		.WithStepSearcherSetters<BivalueUniversalGraveStepSearcher>(static s => s.SearchExtendedTypes = true)
		.WithStepSearcherSetters<ReverseBivalueUniversalGraveStepSearcher>(static s => { s.MaxSearchingEmptyCellsCount = 2; s.AllowPartiallyUsedTypes = true; })
		.WithStepSearcherSetters<AlmostLockedSetsXzStepSearcher>(static s => { s.AllowCollision = true; s.AllowLoopedPatterns = true; })
		.WithStepSearcherSetters<AlmostLockedSetsXyWingStepSearcher>(static s => s.AllowCollision = true)
		.WithStepSearcherSetters<RegularWingStepSearcher>(static s => s.MaxSearchingPivotsCount = 5)
		.WithStepSearcherSetters<TemplateStepSearcher>(static s => s.TemplateDeleteOnly = false)
		.WithStepSearcherSetters<ComplexFishStepSearcher>(static s => s.MaxSize = 5)
		.WithStepSearcherSetters<BowmanBingoStepSearcher>(static s => s.MaxLength = 64)
		.WithStepSearcherSetters<AlmostLockedCandidatesStepSearcher>(static s => s.CheckAlmostLockedQuadruple = false)
		.WithStepSearcherSetters<AlignedExclusionStepSearcher>(static s => s.MaxSearchingSize = 3);


	/// <summary>
	/// Try to check the technique that can make <paramref name="previous"/> to be changed into the state for <paramref name="current"/>.
	/// </summary>
	/// <param name="this">The step collector.</param>
	/// <param name="previous">The first sudoku grid puzzle to be checked. The value is at the previous state.</param>
	/// <param name="current">The second sudoku grid puzzle to be checked. The value is at the current state.</param>
	/// <param name="stepFilter">
	/// A filter method that checks whether a <see cref="Step"/> instance is valid. The value can be <see langword="null"/>
	/// if you don't want to make any extra checking.
	/// </param>
	/// <param name="steps">
	/// The found steps describing the changing making the <paramref name="previous"/> to be changed into <paramref name="current"/>.
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the operation is successful. Values are:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The diff change is found.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The diff change is not found, but two grids are valid to be checked.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>
	/// Arguments <paramref name="previous"/> and <paramref name="current"/> are invalid that makes the operation failed to be checked.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	[return: NotNullIfNotNull(nameof(steps))]
	public static bool? TryGetDiffTechnique(
		this StepCollector @this,
		scoped ref readonly Grid previous,
		scoped ref readonly Grid current,
		StepFilter? stepFilter,
		out Step[]? steps
	)
	{
		if (!previous.IsValid || !current.IsValid)
		{
			goto ReturnNull;
		}

		// Check the validity of the current grid. A valid grid can only produce changes:
		//
		//     1) An extra assignment
		//     2) Some disappeared candidate
		//
		// Otherwise, the grid is an invalid grid pattern.
		var assignmentConclusion = default(Conclusion?);
		var eliminationConclusions = new List<Conclusion>();
		for (var cell = 0; cell < 81; cell++)
		{
			switch (previous.GetState(cell), current.GetState(cell))
			{
				case var (a, b) when a == b && previous[cell] == current[cell]:
				{
					continue;
				}
				case (CellState.Empty, CellState.Empty):
				{
					// Eliminations may exist here.
					var left = previous.GetCandidates(cell);
					var right = current.GetCandidates(cell);
					if ((left & right) != right || assignmentConclusion is not null)
					{
						goto ReturnNull;
					}

					eliminationConclusions.AddRange(from digit in (Mask)(left & ~right) select new Conclusion(Elimination, cell, digit));
					break;
				}
				case (CellState.Empty, CellState.Modifiable):
				{
					// An assignment.
					var setDigit = current.GetDigit(cell);
					if ((previous.GetCandidates(cell) >> setDigit & 1) == 0 || eliminationConclusions.Count != 0)
					{
						goto ReturnNull;
					}

					assignmentConclusion = new(Assignment, cell, setDigit);
					break;
				}
				default:
				{
					// Invalid.
					goto ReturnNull;
				}
			}
		}

		// Merge conclusion to be matched.
		var conclusions = (ConclusionCollection)([
			.. (ReadOnlySpan<Conclusion>)(assignmentConclusion is { } c ? [c] : []),
			.. eliminationConclusions
		]);
		var resultSteps = new List<Step>();
		foreach (var s in StepCollector.Collect(in previous)!)
		{
			if (conclusions == [.. s.Conclusions] && (stepFilter?.Invoke(s) ?? true))
			{
				resultSteps.Add(s);
			}
		}

		steps = [.. resultSteps];
		return true;

	ReturnNull:
		steps = null;
		return null;
	}
}
