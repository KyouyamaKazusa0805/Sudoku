namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the true candidates in bi-value universal grave.
/// </summary>
/// <typeparam name="TStep">The type of the step.</typeparam>
public abstract class BivalueUniversalGraveTrueCandidateFactor<TStep> : Factor
	where TStep : Step, ITrueCandidatesTrait
{
	/// <inheritdoc/>
	public sealed override string FormulaString => "A002024({0}.Count)";

	/// <inheritdoc/>
	public sealed override string[] ParameterNames => [nameof(ITrueCandidatesTrait.TrueCandidates)];

	/// <inheritdoc/>
	public sealed override Type ReflectedStepType => typeof(TStep);

	/// <inheritdoc/>
	public sealed override Func<Step, int?> Formula
		=> static step => step switch
		{
			TStep { TrueCandidates.Count: var count } => A002024(count),
			_ => null
		};
}
