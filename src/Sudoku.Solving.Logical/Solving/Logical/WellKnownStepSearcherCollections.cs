namespace Sudoku.Solving.Logical;

/// <summary>
/// Represents a list of <see cref="StepSearcherCollection"/>s.
/// </summary>
/// <seealso cref="StepSearcherCollection"/>
public static class WellKnownStepSearcherCollections
{
	/// <summary>
	/// Indicates a <see cref="StepSearcherCollection"/> instance that only contains SSTS step searchers.
	/// </summary>
	public static readonly StepSearcherCollection SimpleSudokuTechniqueSet = new IStepSearcher[]
	{
		new SingleStepSearcher(),
		new LockedCandidatesStepSearcher(),
		new SubsetStepSearcher()
	};

	/// <summary>
	/// Indicates a <see cref="StepSearcherCollection"/> instance that contains advanced techniques.
	/// </summary>
	public static readonly StepSearcherCollection AdvancedTechniqueSet = new IStepSearcher[]
	{
		new SingleStepSearcher(),
		new LockedCandidatesStepSearcher(),
		new SubsetStepSearcher(),
		new NormalFishStepSearcher(),
		new TwoStrongLinksStepSearcher(),
		new RegularWingStepSearcher(),
		new WWingStepSearcher(),
		new MultiBranchWWingStepSearcher(),
		new UniqueRectangleStepSearcher(),
		new AlmostLockedCandidatesStepSearcher(),
		new SueDeCoqStepSearcher(),
		new SueDeCoq3DimensionStepSearcher(),
		new UniqueLoopStepSearcher(),
		new ExtendedRectangleStepSearcher(),
		new EmptyRectangleStepSearcher(),
		new UniquePolygonStepSearcher(),
		new BivalueUniversalGraveStepSearcher(),
		new AlmostLockedSetsXzStepSearcher()
	};
}
