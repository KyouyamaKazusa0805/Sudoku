namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="BivalueUniversalGraveType3Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="BivalueUniversalGraveType3Step"/>
public sealed class BivalueUniversalGraveSubsetSizeFactor(StepSearcherOptions options) :
	AlmostSubsetSizeFactor<BivalueUniversalGraveType3Step>(options);
