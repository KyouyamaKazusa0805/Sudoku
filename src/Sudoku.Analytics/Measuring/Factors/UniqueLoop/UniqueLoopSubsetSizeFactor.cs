namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the subset size appeared in <see cref="UniqueLoopType3Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueLoopType3Step"/>
public sealed class UniqueLoopSubsetSizeFactor(StepSearcherOptions options) : AlmostSubsetSizeFactor<UniqueLoopType3Step>(options);
