namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="QiuDeadlyPatternType3Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="QiuDeadlyPatternType3Step"/>
public sealed class QiuDeadlyPatternSubsetSizeFactor(StepSearcherOptions options) : AlmostSubsetSizeFactor<QiuDeadlyPatternType3Step>(options);
