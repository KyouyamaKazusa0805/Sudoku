namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures size property for <see cref="AlmostLockedCandidatesStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="AlmostLockedCandidatesStep"/>
public sealed class AlmostLockedCandidatesSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			2 => 0,
			3 => 7,
			4 => 12
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(AlmostLockedCandidatesStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(AlmostLockedCandidatesStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			AlmostLockedCandidatesStep { Size: var size } => size switch { 2 => 0, 3 => 7, 4 => 12 },
			_ => null
		};
}
