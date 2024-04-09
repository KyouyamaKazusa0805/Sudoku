namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of guardians appeared in external unique rectangle types.
/// </summary>
/// <typeparam name="TStep">The type of the step.</typeparam>
public abstract class RectangleExternalGuardianFactor<TStep> : Factor
	where TStep : Step, IGuardianTrait
{
	/// <inheritdoc/>
	public sealed override string FormulaString => "A004526({0}.Count)";

	/// <inheritdoc/>
	public sealed override string[] ParameterNames => [nameof(IGuardianTrait.GuardianCells)];

	/// <inheritdoc/>
	public sealed override Type ReflectedStepType => typeof(TStep);

	/// <inheritdoc/>
	public sealed override Func<Step, int?> Formula
		=> static step => step switch
		{
			TStep { GuardianCells.Count: var guardiansCount } => A004526(guardiansCount),
			_ => null
		};
}
