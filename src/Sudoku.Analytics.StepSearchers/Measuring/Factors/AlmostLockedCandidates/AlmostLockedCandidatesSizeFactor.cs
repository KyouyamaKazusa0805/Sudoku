namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures size property for <see cref="AlmostLockedCandidatesStep"/>.
/// </summary>
/// <seealso cref="AlmostLockedCandidatesStep"/>
public sealed class AlmostLockedCandidatesSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(AlmostLockedCandidatesStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(AlmostLockedCandidatesStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (int)args![0]! switch { 2 => 0, 3 => 7, 4 => 12 };
}
