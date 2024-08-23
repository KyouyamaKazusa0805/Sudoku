namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes whether a rectangle is incomplete.
/// </summary>
public sealed class UniqueRectangleExternalAlmostLockedSetsXzGuardianIsIncompleteFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleExternalAlmostLockedSetsXzStep.IsIncomplete)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalAlmostLockedSetsXzStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (bool)args![0]! ? 1 : 0;
}
