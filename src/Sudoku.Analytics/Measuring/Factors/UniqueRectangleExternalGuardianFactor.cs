namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of guardians appeared in external unique rectangle types.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class UniqueRectangleExternalGuardianFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "A004526({0}.Count)";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleExternalType1Or2Step.GuardianCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalType1Or2Step);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			UniqueRectangleExternalType1Or2Step { GuardianCells.Count: var guardiansCount } => A004526(guardiansCount),
			_ => null
		};
}
