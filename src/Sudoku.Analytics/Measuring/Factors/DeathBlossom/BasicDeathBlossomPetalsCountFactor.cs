namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed class BasicDeathBlossomPetalsCountFactor(StepSearcherOptions options) :
	DeathBlossomPetalsCountFactor<DeathBlossomStep, NormalBlossomBranchCollection, Digit>(options);
