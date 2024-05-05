namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures length of <see cref="BivalueOddagonStep"/>.
/// </summary>
/// <seealso cref="BivalueOddagonStep"/>
public sealed class BivalueOddagonLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BivalueOddagonStep.LoopCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueOddagonStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((CellMap)args![0]!).Count);
}
