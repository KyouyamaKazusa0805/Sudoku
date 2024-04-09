namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of guardians appeared in <see cref="UniqueRectangleExternalAlmostLockedSetsXzStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleExternalAlmostLockedSetsXzStep"/>
public sealed class UniqueRectangleExternalAlmostLockedSetsXzGuardianFactor(StepSearcherOptions options) :
	RectangleExternalGuardianFactor<UniqueRectangleExternalAlmostLockedSetsXzStep>(options);
