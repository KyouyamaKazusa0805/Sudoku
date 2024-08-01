namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian appeared in <see cref="UniqueRectangleExternalWWingStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleExternalWWingStep"/>
public sealed class UniqueRectangleExternalWWingGuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IGuardianTrait.GuardianCellsCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalWWingStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => OeisSequences.A004526((int)args![0]!);
}
