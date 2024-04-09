namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the rectangle is incomplete.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class UniqueRectangleExternalTurbotFishIsIncompleteFactor(StepSearcherOptions options) :
	RectangleIsIncompleteFactor<UniqueRectangleExternalTurbotFishStep>(options);
