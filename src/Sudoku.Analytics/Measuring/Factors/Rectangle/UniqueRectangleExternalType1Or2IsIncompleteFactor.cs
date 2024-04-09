namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the external unique rectangle is incomplete.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class UniqueRectangleExternalType1Or2IsIncompleteFactor(StepSearcherOptions options) :
	RectangleIsIncompleteFactor<UniqueRectangleExternalType3Step>(options);
