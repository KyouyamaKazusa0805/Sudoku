namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian appeared in <see cref="UniqueRectangleExternalWWingStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleExternalWWingStep"/>
public sealed class UniqueRectangleExternalWWingGuardianFactor(StepSearcherOptions options) :
	RectangleExternalGuardianFactor<UniqueRectangleExternalWWingStep>(options);
