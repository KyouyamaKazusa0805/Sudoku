namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the subset appeared in <see cref="DirectSubsetStep"/> is locked.
/// </summary>
/// <seealso cref="DirectSubsetStep"/>
public sealed class DirectSubsetIsLockedFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			true => {1} switch {{ true => {2} switch {{ 2 => -10, 3 => -11 }}, false => 1, _ => 0 }},
			_ => {1} switch {{ true => {2} switch {{ 2 => -12, 3 => -13 }}, _ => 0 }}
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames
		=> [nameof(DirectSubsetStep.IsNaked), nameof(DirectSubsetStep.IsLocked), nameof(DirectSubsetStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(DirectSubsetStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			DirectSubsetStep { IsNaked: var isNaked, IsLocked: var isLocked, Size: var size } => isNaked switch
			{
				true => isLocked switch { true => size switch { 2 => -10, 3 => -11 }, false => 1, _ => 0 },
				_ => isLocked switch { true => size switch { 2 => -12, 3 => -13 }, _ => 0 }
			},
			_ => null
		};
}
