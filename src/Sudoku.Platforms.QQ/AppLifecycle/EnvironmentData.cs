namespace Sudoku.Platforms.QQ.AppLifecycle;

/// <summary>
/// Provides with environment data.
/// </summary>
internal static class EnvironmentData
{
	/// <summary>
	/// The random number generator.
	/// </summary>
	public static readonly Random Rng = new();

	/// <summary>
	/// The generator.
	/// </summary>
	public static readonly PatternBasedPuzzleGenerator Generator = new();

	/// <summary>
	/// The solver.
	/// </summary>
	public static readonly LogicalSolver Solver = new()
	{
		SingleStepSearcher_EnableFullHouse = true,
		SingleStepSearcher_EnableLastDigit = true,
		SingleStepSearcher_HiddenSinglesInBlockFirst = true,
		UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles = true,
		UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles = true,
		BivalueUniversalGraveStepSearcher_SearchExtendedTypes = true,
		AlmostLockedSetsXyWingStepSearcher_AllowCollision = true,
		AlmostLockedSetsXzStepSearcher_AllowCollision = true,
		AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns = true,
		AlternatingInferenceChainStepSearcher_MaxCapacity = 3000,
		RegularWingStepSearcher_MaxSize = 5,
		TemplateStepSearcher_TemplateDeleteOnly = false,
		ComplexFishStepSearcher_MaxSize = 5,
		BowmanBingoStepSearcher_MaxLength = 64,
		AlmostLockedCandidatesStepSearcher_CheckAlmostLockedQuadruple = false,
		IgnoreSlowAlgorithms = false
	};

	/// <summary>
	/// Indicates the auto filler.
	/// </summary>
	public static readonly DefaultAutoFiller GridAutoFiller = new();

	/// <summary>
	/// The internal running context.
	/// </summary>
	internal static readonly ConcurrentDictionary<string, BotRunningContext> RunningContexts = new();
}
