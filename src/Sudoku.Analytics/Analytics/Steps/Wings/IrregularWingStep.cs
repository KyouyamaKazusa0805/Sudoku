namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Irregular Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class IrregularWingStep(Conclusion[] conclusions, View[]? views, StepGathererOptions options) :
	WingStep(conclusions, views, options)
{
	/// <summary>
	/// Indicates whether the pattern is symmetric.
	/// </summary>
	public abstract bool IsSymmetricPattern { get; }

	/// <summary>
	/// Indicates whether the pattern is grouped.
	/// </summary>
	public abstract bool IsGrouped { get; }

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_IrregularWingIsGroupedFactor",
				[nameof(IsGrouped)],
				GetType(),
				static args => (bool)args![0]! ? 1 : 0
			)
		];
}
