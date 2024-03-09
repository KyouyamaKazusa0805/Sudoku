namespace Sudoku.Analytics;

/// <summary>
/// Represents a list of <see cref="Analyzer"/> instances that are already configured.
/// </summary>
/// <seealso cref="Analyzer"/>
public static class Analyzers
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
			.WithStepSearcherSetters<NormalFishStepSearcher>(static s => { s.DisableFinnedOrSashimiXWing = false; s.AllowSiamese = true; })
			.WithStepSearcherSetters<RegularWingStepSearcher>(static s => s.MaxSearchingPivotsCount = 9)
			.WithStepSearcherSetters<ReverseBivalueUniversalGraveStepSearcher>(static s => { s.MaxSearchingEmptyCellsCount = 4; s.AllowPartiallyUsedTypes = true; })
			.WithStepSearcherSetters<ComplexFishStepSearcher>(static s => { s.MaxSize = 7; s.AllowSiamese = true; })
			.WithStepSearcherSetters<XyzRingStepSearcher>(static s => s.AllowSiamese = false)
			.WithStepSearcherSetters<BowmanBingoStepSearcher>(static s => s.MaxLength = 64)
			.WithStepSearcherSetters<AlignedExclusionStepSearcher>(static s => s.MaxSearchingSize = 5);

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that has some extra configuration, suitable for a whole analysis lifecycle.
	/// </summary>
	public static Analyzer Balanced
		=> Default
			.WithAlgorithmLimits(false, true)
			.WithStepSearcherSetters<SingleStepSearcher>(static s => { s.EnableFullHouse = true; s.EnableLastDigit = true; s.HiddenSinglesInBlockFirst = true; s.UseIttoryuMode = false; })
			.WithStepSearcherSetters<NormalFishStepSearcher>(static s => { s.DisableFinnedOrSashimiXWing = false; s.AllowSiamese = false; })
			.WithStepSearcherSetters<UniqueRectangleStepSearcher>(static s => { s.AllowIncompleteUniqueRectangles = true; s.SearchForExtendedUniqueRectangles = true; })
			.WithStepSearcherSetters<BivalueUniversalGraveStepSearcher>(static s => s.SearchExtendedTypes = true)
			.WithStepSearcherSetters<ReverseBivalueUniversalGraveStepSearcher>(static s => { s.MaxSearchingEmptyCellsCount = 2; s.AllowPartiallyUsedTypes = true; })
			.WithStepSearcherSetters<AlmostLockedSetsXzStepSearcher>(static s => { s.AllowCollision = true; s.AllowLoopedPatterns = true; })
			.WithStepSearcherSetters<AlmostLockedSetsXyWingStepSearcher>(static s => s.AllowCollision = true)
			.WithStepSearcherSetters<RegularWingStepSearcher>(static s => s.MaxSearchingPivotsCount = 5)
			.WithStepSearcherSetters<TemplateStepSearcher>(static s => s.TemplateDeleteOnly = false)
			.WithStepSearcherSetters<ComplexFishStepSearcher>(static s => { s.MaxSize = 5; s.AllowSiamese = false; })
			.WithStepSearcherSetters<XyzRingStepSearcher>(static s => s.AllowSiamese = false)
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
			.WithStepSearchers(
				new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true, HiddenSinglesInBlockFirst = true, UseIttoryuMode = false },
				new LockedSubsetStepSearcher(),
				new LockedCandidatesStepSearcher(),
				new NormalSubsetStepSearcher()
			)
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that only supports for techniques used in Sudoku Explainer.
	/// </summary>
	public static Analyzer SudokuExplainer
		=> Default
			.WithStepSearchers(
				new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true, HiddenSinglesInBlockFirst = true, UseIttoryuMode = false },
				new LockedSubsetStepSearcher(),
				new LockedCandidatesStepSearcher(),
				new NormalSubsetStepSearcher(),
				new NormalFishStepSearcher { AllowSiamese = false },
				new RegularWingStepSearcher { MaxSearchingPivotsCount = 3 },
				new UniqueRectangleStepSearcher { AllowIncompleteUniqueRectangles = false, SearchForExtendedUniqueRectangles = false },
				new UniqueLoopStepSearcher(),
				new BivalueUniversalGraveStepSearcher { SearchExtendedTypes = false },
				new AlignedExclusionStepSearcher { MaxSearchingSize = 3 },
				new NonMultipleChainingStepSearcher(),
				new MultipleChainingStepSearcher()
			)
			.WithAlgorithmLimits(false, false)
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });
}
