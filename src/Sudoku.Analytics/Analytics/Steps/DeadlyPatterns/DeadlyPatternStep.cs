namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Deadly Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class DeadlyPatternStep(Conclusion[] conclusions, View[]? views, StepSearcherOptions options) : Step(conclusions, views, options)
{
	/// <summary>
	/// Indicates whether the deadly pattern only uses bi-value cells.
	/// </summary>
	public abstract bool OnlyUseBivalueCells { get; }
}
