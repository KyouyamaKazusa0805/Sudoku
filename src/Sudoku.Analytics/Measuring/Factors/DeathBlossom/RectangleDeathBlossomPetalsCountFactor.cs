namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed class RectangleDeathBlossomPetalsCountFactor(StepSearcherOptions options) :
	DeathBlossomPetalsCountFactor<RectangleDeathBlossomStep, RectangleBlossomBranchCollection, Candidate>(options);
