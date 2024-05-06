namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the true candidates appeared in <see cref="BivalueUniversalGraveType2Step"/>.
/// </summary>
/// <seealso cref="BivalueUniversalGraveType2Step"/>
public sealed partial class BivalueUniversalGraveType2TrueCandidateFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueUniversalGraveType2Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => OeisSequences.A002024((int)args![0]!);
}
