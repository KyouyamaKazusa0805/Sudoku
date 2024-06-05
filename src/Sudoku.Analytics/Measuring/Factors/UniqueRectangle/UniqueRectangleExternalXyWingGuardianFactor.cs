namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian appeared in <see cref="UniqueRectangleExternalXyWingStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleExternalXyWingStep"/>
public sealed class UniqueRectangleExternalXyWingGuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IGuardianTrait.GuardianCellsCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalXyWingStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => OeisSequences.A004526((int)args![0]!);
}
