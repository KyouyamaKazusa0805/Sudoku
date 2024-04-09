namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of guardians appeared in <see cref="UniqueRectangleExternalType1Or2Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleExternalType1Or2Step"/>
public sealed class UniqueRectangleExternalType1Or2GuardianFactor(StepSearcherOptions options) :
	RectangleExternalGuardianFactor<UniqueRectangleExternalType1Or2Step>(options);
