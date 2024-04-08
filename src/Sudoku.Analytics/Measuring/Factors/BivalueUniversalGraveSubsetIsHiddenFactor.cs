namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the subset appeare in <see cref="BivalueUniversalGraveType3Step"/> is hidden.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="BivalueUniversalGraveType3Step"/>
public sealed class BivalueUniversalGraveSubsetIsHiddenFactor(StepSearcherOptions options) :
	AlmostSubsetIsHiddenFactor<BivalueUniversalGraveType3Step>(options);
