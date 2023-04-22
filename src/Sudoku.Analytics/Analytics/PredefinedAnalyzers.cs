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
	/// <item><see cref="SubsetStepSearcher"/></item>
	/// <item></item>
	/// </list>
	/// </summary>
	/// <seealso cref="SingleStepSearcher"/>
	/// <seealso cref="LockedCandidatesStepSearcher"/>
	/// <seealso cref="SubsetStepSearcher"/>
	public static Analyzer SstsOnly
		=> Default
			.WithStepSearchers(new StepSearcher[] { new SingleStepSearcher(), new LockedCandidatesStepSearcher(), new SubsetStepSearcher() })
			.WithStepSearcherSetters<SingleStepSearcher>(
				static s =>
				{
					s.EnableFullHouse = true;
					s.EnableLastDigit = true;
					s.HiddenSinglesInBlockFirst = true;
					s.UseIttoryuMode = false;
				});
}
