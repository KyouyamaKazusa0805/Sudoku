using Sudoku.Analytics.Configuration;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Single</b> or <b>Last Digit</b> (for special cases) technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="house">Indicates the house where the current Hidden Single technique forms.</param>
/// <param name="eliminatedCellsCount">The total eliminated cells.</param>
/// <param name="eliminatedHouses">The total eliminated houses.</param>
public sealed class LastDigitStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	House house,
	int[] eliminatedCellsCount,
	House[] eliminatedHouses
) : HiddenSingleStep(conclusions, views, options, cell, digit, house, true, eliminatedCellsCount, eliminatedHouses);
