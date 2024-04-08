namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="UniqueMatrixType3Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueMatrixType3Step"/>
public sealed class UniqueMatrixSubsetSizeFactor(StepSearcherOptions options) : AlmostSubsetSizeFactor<UniqueMatrixType3Step>(options);
