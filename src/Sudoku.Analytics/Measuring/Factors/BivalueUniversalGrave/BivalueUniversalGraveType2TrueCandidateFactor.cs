namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the true candidates appeared in <see cref="BivalueUniversalGraveType2Step"/>.
/// </summary>
/// <seealso cref="BivalueUniversalGraveType2Step"/>
public sealed class BivalueUniversalGraveType2TrueCandidateFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ITrueCandidatesTrait.TrueCandidates)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueUniversalGraveType2Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024(((CellMap)args![0]!).Count);
}
