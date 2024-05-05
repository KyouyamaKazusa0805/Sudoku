namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures value cell existence for <see cref="AlmostLockedCandidatesStep"/>.
/// </summary>
/// <seealso cref="AlmostLockedCandidatesStep"/>
public sealed partial class AlmostLockedCandidatesValueCellExistenceFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames
		=> [nameof(AlmostLockedCandidatesStep.HasValueCell), nameof(AlmostLockedCandidatesStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(AlmostLockedCandidatesStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? (int)args![1]! switch { 2 or 3 => 1, 4 => 2 } : 0;
}
