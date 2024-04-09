namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="BorescoperDeadlyPatternType3Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="BorescoperDeadlyPatternType3Step"/>
public sealed class BorescoperDeadlyPatternSubsetSizeFactor(StepSearcherOptions options) :
	AlmostSubsetSizeFactor<BorescoperDeadlyPatternType3Step>(options);
