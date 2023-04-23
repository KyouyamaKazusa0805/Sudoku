namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="digit1">Indicates the first digit used in the conjugate.</param>
/// <param name="digit2">Indicates the second digit used in the conjugate.</param>
/// <param name="conjugateHouse">Indicates the cells that describes the generalized conjugate pair.</param>
public sealed partial class UniqueMatrixType4Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] Digit digit1,
	[PrimaryConstructorParameter] Digit digit2,
	[PrimaryConstructorParameter] scoped in CellMap conjugateHouse
) : UniqueMatrixStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ConjStr, Digit1Str, Digit2Str } },
			{ "zh", new[] { ConjStr, Digit1Str, Digit2Str, DigitsStr, CellsStr } }
		};

	private string ConjStr => ConjugateHouse.ToString();

	private string Digit1Str => (Digit1 + 1).ToString();

	private string Digit2Str => (Digit2 + 1).ToString();
}
