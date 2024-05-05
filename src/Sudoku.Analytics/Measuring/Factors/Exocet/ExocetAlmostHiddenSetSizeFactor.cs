namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of almost hidden set appeared in <see cref="JuniorExocetMirrorAlmostHiddenSetStep"/>.
/// </summary>
/// <seealso cref="JuniorExocetMirrorAlmostHiddenSetStep"/>
public sealed class ExocetAlmostHiddenSetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(JuniorExocetMirrorAlmostHiddenSetStep.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(JuniorExocetMirrorAlmostHiddenSetStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024((int)args![0]!);
}
