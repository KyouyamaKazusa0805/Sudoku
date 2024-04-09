namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of subset appeared in <see cref="UniqueRectangleExternalType3Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleExternalType3Step"/>
public sealed class UniqueRectangleExternalSubsetSizeFactor(StepSearcherOptions options) :
	AlmostSubsetSizeFactor<UniqueRectangleExternalType3Step>(options);
