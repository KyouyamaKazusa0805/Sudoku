namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian in <see cref="GuardianStep"/>.
/// </summary>
/// <seealso cref="GuardianStep"/>
public sealed class GuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "A004526({0} + A004526({1}))";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(GuardianStep.LoopCells), nameof(GuardianStep.Guardians)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(GuardianStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			GuardianStep { LoopCells.Count: var loopCellsCount, Guardians.Count: var guardiansCount }
				=> A004526(loopCellsCount + A004526(guardiansCount)),
			_ => null
		};
}
