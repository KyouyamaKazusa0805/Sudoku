namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether an <see cref="IrregularWingStep"/> is grouped.
/// </summary>
/// <seealso cref="IrregularWingStep"/>
public sealed class IrregularWingIsGroupedFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "{0} ? 1 : 0";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IrregularWingStep.IsGrouped)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(IrregularWingStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			IrregularWingStep { IsGrouped: var isGrouped } => isGrouped ? 1 : 0,
			_ => null
		};
}
