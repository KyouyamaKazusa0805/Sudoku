namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of guardians appeared in <see cref="UniqueRectangleExternalType1Or2Step"/>.
/// </summary>
/// <seealso cref="UniqueRectangleExternalType1Or2Step"/>
public sealed partial class UniqueRectangleExternalType1Or2GuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleExternalType1Or2Step.GuardianCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalType1Or2Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((CellMap)args![0]!).Count);
}
