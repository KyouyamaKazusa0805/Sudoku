namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed class NTimesAlmostLockedSetsDeathBlossomPetalsCountFactor(StepSearcherOptions options) :
	DeathBlossomPetalsCountFactor<NTimesAlmostLockedSetDeathBlossomStep, NTimesAlmostLockedSetsBlossomBranchCollection, CandidateMap>(options);
