namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether a rectangle is incomplete.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class UniqueRectangleExternalAlmostLockedSetsXzGuardianIsIncompleteFactor(StepSearcherOptions options) :
	RectangleIsIncompleteFactor<UniqueRectangleExternalAlmostLockedSetsXzStep>(options);
