namespace Sudoku.Solving.Logical;

/// <summary>
/// Provides with some commonly-used <see cref="LogicalSolver"/> instances.
/// </summary>
/// <remarks><b>
/// You shouldn't modify any property from instances provided by this type because <see cref="LogicalSolver"/> is a reference type.
/// </b></remarks>
/// <seealso cref="LogicalSolver"/>
public static class CommonLogicalSolvers
{
	/// <summary>
	/// Indicates a <see cref="LogicalSolver"/> instance that is created by default instantiation behavior.
	/// </summary>
	public static readonly LogicalSolver Default = new();

	/// <summary>
	/// Indicates a <see cref="LogicalSolver"/> instance that only provides steps for simple-sudoku techinques:
	/// <list type="bullet">
	/// <item>Hidden/Naked Singles</item>
	/// <item>Locked Candidates</item>
	/// <item>Hidden/Naked/Locked Subsets &amp; Naked Subsets (+)</item>
	/// </list>
	/// </summary>
	public static readonly LogicalSolver SstsOnly = new()
	{
		SingleStepSearcher_EnableFullHouse = false,
		SingleStepSearcher_EnableLastDigit = false,
		SingleStepSearcher_HiddenSinglesInBlockFirst = false,
		IsFullApplying = true,
		IgnoreSlowAlgorithms = false,
		IgnoreHighAllocationAlgorithms = true,
		CustomSearcherCollection = new IStepSearcher[] { new SingleStepSearcher(), new LockedCandidatesStepSearcher(), new SubsetStepSearcher() }
	};

	/// <summary>
	/// Indicates a <see cref="LogicalSolver"/> instance that has configured some basic options that is suitable
	/// for human to control sudoku puzzles.
	/// </summary>
	public static readonly LogicalSolver Suitable = new()
	{
		SingleStepSearcher_EnableFullHouse = true,
		SingleStepSearcher_EnableLastDigit = true,
		SingleStepSearcher_HiddenSinglesInBlockFirst = true,
		SingleStepSearcher_UseIttoryuMode = false,
		UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles = true,
		UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles = true,
		BivalueUniversalGraveStepSearcher_SearchExtendedTypes = true,
		AlmostLockedSetsXyWingStepSearcher_AllowCollision = true,
		AlmostLockedSetsXzStepSearcher_AllowCollision = true,
		AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns = true,
		RegularWingStepSearcher_MaxSize = 5,
		TemplateStepSearcher_TemplateDeleteOnly = false,
		ComplexFishStepSearcher_MaxSize = 5,
		BowmanBingoStepSearcher_MaxLength = 64,
		AlmostLockedCandidatesStepSearcher_CheckAlmostLockedQuadruple = false,
		IgnoreSlowAlgorithms = false,
		IgnoreHighAllocationAlgorithms = true
	};
}
