namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of guardians appeared in <see cref="UniqueRectangleExternalAlmostLockedSetsXzStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleExternalAlmostLockedSetsXzStep"/>
public sealed class UniqueRectangleExternalAlmostLockedSetsXzGuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleExternalAlmostLockedSetsXzStep.GuardianCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalAlmostLockedSetsXzStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((CellMap)args![0]!).Count);
}
