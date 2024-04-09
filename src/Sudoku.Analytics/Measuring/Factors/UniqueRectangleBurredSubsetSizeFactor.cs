namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a type that describes the size of subset appeared in <see cref="UniqueRectangleBurredSubsetStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleBurredSubsetStep"/>
public sealed class UniqueRectangleBurredSubsetSizeFactor(StepSearcherOptions options) :
	AlmostSubsetSizeFactor<UniqueRectangleBurredSubsetStep>(options);
