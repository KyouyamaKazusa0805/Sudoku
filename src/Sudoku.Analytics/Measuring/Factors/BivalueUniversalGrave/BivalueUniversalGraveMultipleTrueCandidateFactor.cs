namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the true candidates appeared in <see cref="BivalueUniversalGraveMultipleStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="BivalueUniversalGraveMultipleStep"/>
public sealed class BivalueUniversalGraveMultipleTrueCandidateFactor(StepSearcherOptions options) :
	BivalueUniversalGraveTrueCandidateFactor<BivalueUniversalGraveMultipleStep>(options);
