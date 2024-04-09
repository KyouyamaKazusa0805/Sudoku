namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents the size factor used in naked subsets.
/// </summary>
public sealed class NakedSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			2 => 0,
			3 => 6,
			4 => 20
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NakedSubsetStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NakedSubsetStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			NakedSubsetStep { Size: var size } => size switch { 2 => 0, 3 => 6, 4 => 20 },
			_ => null
		};
}
