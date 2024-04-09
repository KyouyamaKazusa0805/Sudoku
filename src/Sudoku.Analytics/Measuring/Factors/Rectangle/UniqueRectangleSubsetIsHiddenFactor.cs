namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether a subset appeared in <see cref="UniqueRectangleType3Step"/> is hidden.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleType3Step"/>
public sealed class UniqueRectangleSubsetIsHiddenFactor(StepSearcherOptions options) :
	AlmostSubsetIsHiddenFactor<UniqueRectangleType3Step>(options);
