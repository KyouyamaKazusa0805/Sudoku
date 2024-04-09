namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of a <see cref="NormalFishStep"/>.
/// </summary>
/// <seealso cref="NormalFishStep"/>
public sealed class NormalFishSizeFactor : Factor
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
	public override string[] ParameterNames => [nameof(NormalFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NormalFishStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			NormalFishStep { Size: var size } => size switch { 2 => 0, 3 => 6, 4 => 20 },
			_ => null
		};
}
