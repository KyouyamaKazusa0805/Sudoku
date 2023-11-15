using Sudoku.Analytics.StepSearchers;

namespace Sudoku.Analytics;

/// <summary>
/// Represents a list of <see cref="Analyzer"/> instances that are already configured.
/// </summary>
/// <seealso cref="Analyzer"/>
public static class PredefinedAnalyzers
{
	/// <summary>
	/// Indicates the default <see cref="Analyzer"/> instance that has no extra configuration.
	/// </summary>
	public static Analyzer Default => new();

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that all possible <see cref="StepSearcher"/> instances are included.
	/// </summary>
	public static Analyzer AllIn
		=> Balanced
			.WithAlgorithmLimits(false, false)
			.WithStepSearcherSetters<RegularWingStepSearcher>(static s => s.MaxSearchingPivotsCount = 9)
			.WithStepSearcherSetters<ReverseBivalueUniversalGraveStepSearcher>(static s => { s.MaxSearchingEmptyCellsCount = 4; s.AllowPartiallyUsedTypes = true; })
			.WithStepSearcherSetters<ComplexFishStepSearcher>(static s => s.MaxSize = 7)
			.WithStepSearcherSetters<BowmanBingoStepSearcher>(static s => s.MaxLength = 64)
			.WithStepSearcherSetters<AlignedExclusionStepSearcher>(static s => s.MaxSearchingSize = 5);

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that has some extra configuration which are suitable for a whole analysis lifecycle.
	/// </summary>
	public static Analyzer Balanced
		=> Default
			.WithAlgorithmLimits(false, true)
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
	/// Indicates an <see cref="Analyzer"/> instance that only contains SSTS techniques:
	/// <list type="bullet">
	/// <item><see cref="SingleStepSearcher"/></item>
	/// <item><see cref="LockedCandidatesStepSearcher"/></item>
	/// <item><see cref="LockedSubsetStepSearcher"/></item>
	/// <item><see cref="NormalSubsetStepSearcher"/></item>
	/// </list>
	/// </summary>
	/// <seealso cref="SingleStepSearcher"/>
	/// <seealso cref="LockedCandidatesStepSearcher"/>
	/// <seealso cref="LockedSubsetStepSearcher"/>
	/// <seealso cref="NormalSubsetStepSearcher"/>
	public static Analyzer SstsOnly
		=> Default
			.WithStepSearchers([new SingleStepSearcher(), new LockedSubsetStepSearcher(), new LockedCandidatesStepSearcher(), new NormalSubsetStepSearcher()])
			.WithStepSearcherSetters<SingleStepSearcher>(static s =>
			{
				s.EnableFullHouse = true;
				s.EnableLastDigit = true;
				s.HiddenSinglesInBlockFirst = true;
				s.UseIttoryuMode = false;
			});

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that only supports for techniques used in Sudoku Explainer.
	/// </summary>
	public static Analyzer SudokuExplainer
		=> Default
			.WithStepSearchers([
				new SingleStepSearcher(),
				new LockedSubsetStepSearcher(),
				new LockedCandidatesStepSearcher(),
				new NormalSubsetStepSearcher(),
				new RegularWingStepSearcher(),
				new UniqueRectangleStepSearcher(),
				new UniqueLoopStepSearcher(),
				new BivalueUniversalGraveStepSearcher(),
				new AlignedExclusionStepSearcher(),
				new NonMultipleChainingStepSearcher(),
				new MultipleChainingStepSearcher()
			])
			.WithAlgorithmLimits(false, false)
			.WithStepSearcherSetters<SingleStepSearcher>(static s => { s.EnableFullHouse = true; s.EnableLastDigit = true; s.HiddenSinglesInBlockFirst = true; s.UseIttoryuMode = false; })
			.WithStepSearcherSetters<UniqueRectangleStepSearcher>(static s => { s.AllowIncompleteUniqueRectangles = false; s.SearchForExtendedUniqueRectangles = false; })
			.WithStepSearcherSetters<BivalueUniversalGraveStepSearcher>(static s => s.SearchExtendedTypes = false)
			.WithStepSearcherSetters<RegularWingStepSearcher>(static s => s.MaxSearchingPivotsCount = 3)
			.WithStepSearcherSetters<AlignedExclusionStepSearcher>(static s => s.MaxSearchingSize = 3);
}
