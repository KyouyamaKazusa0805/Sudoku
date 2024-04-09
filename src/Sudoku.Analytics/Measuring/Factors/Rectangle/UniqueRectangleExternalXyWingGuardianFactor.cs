namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian appeared in <see cref="UniqueRectangleExternalXyWingStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleExternalXyWingStep"/>
public sealed class UniqueRectangleExternalXyWingGuardianFactor(StepSearcherOptions options) :
	RectangleExternalGuardianFactor<UniqueRectangleExternalXyWingStep>(options);
