namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian appeared in <see cref="UniqueRectangleExternalTurbotFishStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleExternalTurbotFishStep"/>
public sealed class UniqueRectangleExternalTurbotFishGuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleExternalTurbotFishStep.GuardianCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalTurbotFishStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((CellMap)args![0]!).Count);
}
