namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of branches appeared in a <see cref="MultiBranchWWingStep"/>.
/// </summary>
/// <seealso cref="MultiBranchWWingStep"/>
public sealed class MultiBranchWWingBranchesCountFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "{0} == 3 ? 3 : 0";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(MultiBranchWWingStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(MultiBranchWWingStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			MultiBranchWWingStep { Size: var size } => size == 3 ? 3 : 0,
			_ => null
		};
}
