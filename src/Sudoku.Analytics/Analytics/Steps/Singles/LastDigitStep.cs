using Sudoku.Analytics.Configuration;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Last Digit</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="house"><inheritdoc/></param>
public sealed class LastDigitStep(Conclusion[] conclusions, View[]? views, StepSearcherOptions options, Cell cell, Digit digit, House house) :
	HiddenSingleStep(conclusions, views, options, cell, digit, house, true, [], [], [], 0, 0, 0, 0, 0);
