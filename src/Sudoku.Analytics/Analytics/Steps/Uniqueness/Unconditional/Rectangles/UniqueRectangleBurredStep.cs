namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Burred</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="code"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public abstract class UniqueRectangleBurredStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Technique code,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(conclusions, views, options, code, digit1, digit2, in cells, isAvoidable, absoluteOffset);
