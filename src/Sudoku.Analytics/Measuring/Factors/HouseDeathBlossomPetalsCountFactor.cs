namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed class HouseDeathBlossomPetalsCountFactor(StepSearcherOptions options) :
	DeathBlossomPetalsCountFactor<HouseDeathBlossomStep, HouseBlossomBranchCollection, House>(options);
