namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="ExtendedRectangleType3Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="ExtendedRectangleType3Step"/>
public sealed class ExtendedRectangleSubsetSizeFactor(StepSearcherOptions options) :
	AlmostSubsetSizeFactor<ExtendedRectangleType3Step>(options);
