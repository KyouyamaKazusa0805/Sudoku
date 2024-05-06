namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of <see cref="UniqueLoopStep"/>.
/// </summary>
/// <seealso cref="UniqueLoopStep"/>
public sealed partial class UniqueLoopLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueLoopStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526((int)args![0]!) - 3;
}
