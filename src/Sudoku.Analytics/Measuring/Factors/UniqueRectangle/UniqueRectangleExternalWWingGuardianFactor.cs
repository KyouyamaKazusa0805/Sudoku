namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian appeared in <see cref="UniqueRectangleExternalWWingStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleExternalWWingStep"/>
public sealed partial class UniqueRectangleExternalWWingGuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IGuardianTrait.GuardianCellsCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalWWingStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526((int)args![0]!);
}
