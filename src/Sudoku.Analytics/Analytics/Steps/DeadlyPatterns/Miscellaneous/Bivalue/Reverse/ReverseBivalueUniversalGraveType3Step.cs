namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Bi-value Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="subsetHouse">Indicates the subset house used.</param>
/// <param name="subsetMask">Indicates the subset digits mask.</param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="emptyCells"><inheritdoc/></param>
public sealed partial class ReverseBivalueUniversalGraveType3Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Digit digit1,
	Digit digit2,
	[Property] House subsetHouse,
	[Property] Mask subsetMask,
	ref readonly CellMap pattern,
	ref readonly CellMap emptyCells
) : ReverseBivalueUniversalGraveStep(conclusions, views, options, digit1, digit2, in pattern, in emptyCells)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | SubsetMask);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [ExtraHouseStr, ExtraDigitsStr]), new(SR.ChineseLanguage, [ExtraHouseStr, ExtraDigitsStr])];

	private string ExtraHouseStr => Options.Converter.HouseConverter(1 << SubsetHouse);

	private string ExtraDigitsStr => Options.Converter.DigitConverter(SubsetMask);
}
