namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>(Grouped) L-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="start">Indicates the start node.</param>
/// <param name="end">Indicates the end node.</param>
/// <param name="digitX">Indicates the digit X.</param>
/// <param name="digitY">Indicates the digit Y.</param>
/// <param name="digitZ">Indicates the digit Z.</param>
/// <param name="midWeakCell1">Indicates the mid cell 1.</param>
/// <param name="midWeakCell2">Indicates the mid cell 2.</param>
public sealed partial class LWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryCosntructorParameter] scoped ref readonly CellMap start,
	[PrimaryCosntructorParameter] scoped ref readonly CellMap end,
	[PrimaryCosntructorParameter] Digit digitX,
	[PrimaryCosntructorParameter] Digit digitY,
	[PrimaryCosntructorParameter] Digit digitZ,
	[PrimaryCosntructorParameter] Cell midWeakCell1,
	[PrimaryCosntructorParameter] Cell midWeakCell2
) : IrregularWingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsGrouped => Start.Count != 1 || End.Count != 1;

	/// <inheritdoc/>
	public override bool IsSymmetricPattern => true;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.8M;

	/// <inheritdoc/>
	public override Technique Code => IsGrouped ? Technique.GroupedLWing : Technique.LWing;
}
