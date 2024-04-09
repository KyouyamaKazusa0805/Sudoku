namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the true candidates appeared in <see cref="BivalueUniversalGraveType2Step"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="BivalueUniversalGraveType2Step"/>
public sealed class BivalueUniversalGraveType2TrueCandidateFactor(StepSearcherOptions options) :
	BivalueUniversalGraveTrueCandidateFactor<BivalueUniversalGraveType2Step>(options);
