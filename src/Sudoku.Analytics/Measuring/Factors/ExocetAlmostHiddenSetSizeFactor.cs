namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of almost hidden set appeared in <see cref="JuniorExocetMirrorAlmostHiddenSetStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="JuniorExocetMirrorAlmostHiddenSetStep"/>
public sealed class ExocetAlmostHiddenSetSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "A002024({0})";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(JuniorExocetMirrorAlmostHiddenSetStep.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(JuniorExocetMirrorAlmostHiddenSetStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			JuniorExocetMirrorAlmostHiddenSetStep { SubsetSize: var size } => A002024(size),
			_ => null
		};
}
