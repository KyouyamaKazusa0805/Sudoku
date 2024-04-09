namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="UniqueRectangleType3Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleType3Step"/>
public sealed class UniqueRectangleSubsetSizeFactor(StepSearcherOptions options) : AlmostSubsetSizeFactor<UniqueRectangleType3Step>(options);
