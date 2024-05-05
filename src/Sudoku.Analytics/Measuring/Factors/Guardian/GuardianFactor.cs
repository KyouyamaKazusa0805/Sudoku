namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian in <see cref="GuardianStep"/>.
/// </summary>
/// <seealso cref="GuardianStep"/>
public sealed class GuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(GuardianStep.LoopCells), nameof(GuardianStep.Guardians)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(GuardianStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => A004526(((CellMap)args![0]!).Count + A004526(((CellMap)args![1]!).Count));
}
