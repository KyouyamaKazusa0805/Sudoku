namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian appeared in <see cref="UniqueRectangleExternalTurbotFishStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleExternalTurbotFishStep"/>
public sealed class UniqueRectangleExternalTurbotFishGuardianFactor(StepSearcherOptions options) :
	RectangleExternalGuardianFactor<UniqueRectangleExternalTurbotFishStep>(options);
