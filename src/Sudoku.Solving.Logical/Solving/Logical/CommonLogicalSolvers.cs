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
	/// Indicates a <see cref="LogicalSolver"/> instance that has configured some basic options that is suitable
	/// for human to control sudoku puzzles.
	/// </summary>
	public static readonly LogicalSolver Suitable = new()
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
		RegularWingStepSearcher_MaxSize = 5,
		TemplateStepSearcher_TemplateDeleteOnly = false,
		ComplexFishStepSearcher_MaxSize = 5,
		BowmanBingoStepSearcher_MaxLength = 64,
		AlmostLockedCandidatesStepSearcher_CheckAlmostLockedQuadruple = false,
		IgnoreSlowAlgorithms = false
	};
}
