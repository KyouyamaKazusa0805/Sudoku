namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="digit1">Indicates the first digit used in the conjugate.</param>
/// <param name="digit2">Indicates the second digit used in the conjugate.</param>
/// <param name="conjugateHouse">Indicates the cells that describes the generalized conjugate pair.</param>
public sealed partial class UniqueMatrixType4Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	scoped ref readonly CellMap cells,
	Mask digitsMask,
	[Data] Digit digit1,
	[Data] Digit digit2,
	[Data] scoped ref readonly CellMap conjugateHouse
) : UniqueMatrixStep(conclusions, views, options, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [DigitsStr, CellsStr, ConjStr, Digit1Str, Digit2Str]),
			new(ChineseLanguage, [ConjStr, Digit1Str, Digit2Str, DigitsStr, CellsStr])
		];

	private string ConjStr => Options.Converter.CellConverter(ConjugateHouse);

	private string Digit1Str => Options.Converter.DigitConverter((Mask)(1 << Digit1));

	private string Digit2Str => Options.Converter.DigitConverter((Mask)(1 << Digit2));
}
