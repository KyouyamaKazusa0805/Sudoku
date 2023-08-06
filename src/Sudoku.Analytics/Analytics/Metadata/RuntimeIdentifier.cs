namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents a list of <see cref="string"/>s indicating the runtime identifier recognized by UI,
/// used by <see cref="RuntimeIdentifierAttribute"/>.
/// </summary>
/// <seealso cref="RuntimeIdentifierAttribute"/>
public static class RuntimeIdentifier
{
	//
	// Step searcher property names
	//
	/// <inheritdoc cref="SingleStepSearcher.EnableFullHouse"/>
	public const string EnableFullHouse = nameof(EnableFullHouse);

	/// <inheritdoc cref="SingleStepSearcher.EnableLastDigit"/>
	public const string EnableLastDigit = nameof(EnableLastDigit);

	/// <inheritdoc cref="SingleStepSearcher.HiddenSinglesInBlockFirst"/>
	public const string HiddenSinglesInBlockFirst = nameof(HiddenSinglesInBlockFirst);

	/// <inheritdoc cref="SingleStepSearcher.UseIttoryuMode"/>
	public const string AnalyzerUseIttoryuMode = nameof(AnalyzerUseIttoryuMode);

	/// <inheritdoc cref="UniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles"/>
	public const string AllowIncompleteUniqueRectangles = nameof(AllowIncompleteUniqueRectangles);

	/// <inheritdoc cref="UniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles"/>
	public const string SearchForExtendedUniqueRectangles = nameof(SearchForExtendedUniqueRectangles);

	/// <inheritdoc cref="BivalueUniversalGraveStepSearcher.SearchExtendedTypes"/>
	public const string SearchExtendedBivalueUniversalGraveTypes = nameof(SearchExtendedBivalueUniversalGraveTypes);

	/// <inheritdoc cref="AlmostLockedSetsXzStepSearcher.AllowCollision"/>
	public const string AllowCollisionOnAlmostLockedSetXzRule = nameof(AllowCollisionOnAlmostLockedSetXzRule);

	/// <inheritdoc cref="AlmostLockedSetsXzStepSearcher.AllowLoopedPatterns"/>
	public const string AllowLoopedPatternsOnAlmostLockedSetXzRule = nameof(AllowLoopedPatternsOnAlmostLockedSetXzRule);

	/// <inheritdoc cref="AlmostLockedSetsXyWingStepSearcher.AllowCollision"/>
	public const string AllowCollisionOnAlmostLockedSetXyWing = nameof(AllowCollisionOnAlmostLockedSetXyWing);

	/// <inheritdoc cref="ReverseBivalueUniversalGraveStepSearcher.AllowPartiallyUsedTypes"/>
	public const string SearchForReverseBugPartiallyUsedTypes = nameof(SearchForReverseBugPartiallyUsedTypes);

	/// <inheritdoc cref="ReverseBivalueUniversalGraveStepSearcher.MaxSearchingEmptyCellsCount"/>
	public const string ReverseBugMaxSearchingEmptyCellsCount = nameof(ReverseBugMaxSearchingEmptyCellsCount);

	/// <inheritdoc cref="AlignedExclusionStepSearcher.MaxSearchingSize"/>
	public const string AlignedExclusionMaxSearchingSize = nameof(AlignedExclusionMaxSearchingSize);

	/// <inheritdoc cref="RegularWingStepSearcher.MaxSearchingPivotsCount"/>
	public const string MaxSizeOfRegularWing = nameof(MaxSizeOfRegularWing);

	/// <inheritdoc cref="ComplexFishStepSearcher.MaxSize"/>
	public const string MaxSizeOfComplexFish = nameof(MaxSizeOfComplexFish);

	/// <inheritdoc cref="TemplateStepSearcher.TemplateDeleteOnly"/>
	public const string TemplateDeleteOnly = nameof(TemplateDeleteOnly);

	/// <inheritdoc cref="BowmanBingoStepSearcher.MaxLength"/>
	public const string BowmanBingoMaxLength = nameof(BowmanBingoMaxLength);

	/// <inheritdoc cref="JuniorExocetStepSearcher.CheckAdvanced"/>
	public const string CheckAdvancedJuniorExocet = nameof(CheckAdvancedJuniorExocet);

	/// <inheritdoc cref="SeniorExocetStepSearcher.CheckAdvanced"/>
	public const string CheckAdvancedSeniorExocet = nameof(CheckAdvancedSeniorExocet);

	/// <inheritdoc cref="AlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple"/>
	public const string CheckAlmostLockedQuadruple = nameof(CheckAlmostLockedQuadruple);

	//
	// Analyzer & Collector property names
	//
	/// <inheritdoc cref="Analyzer.IsFullApplying"/>
	public const string LogicalSolverIsFullApplying = nameof(LogicalSolverIsFullApplying);

	/// <inheritdoc cref="Analyzer.IgnoreSlowAlgorithms"/>
	public const string LogicalSolverIgnoresSlowAlgorithms = nameof(LogicalSolverIgnoresSlowAlgorithms);

	/// <inheritdoc cref="Analyzer.IgnoreHighAllocationAlgorithms"/>
	public const string LogicalSolverIgnoresHighAllocationAlgorithms = nameof(LogicalSolverIgnoresHighAllocationAlgorithms);

	/// <inheritdoc cref="StepCollector.DifficultyLevelMode"/>
	public const string DifficultyLevelMode = nameof(DifficultyLevelMode);

	/// <inheritdoc cref="StepCollector.MaxStepsGathered"/>
	public const string StepGathererMaxStepsGathered = nameof(StepGathererMaxStepsGathered);
}
