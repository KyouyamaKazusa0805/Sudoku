namespace Sudoku.Analytics.Steps.Invalidity;

/// <summary>
/// Provides with a step that is a <b>Bivalue Oddagon Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="loopCells"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="extraCells">Indicates the extra cells used.</param>
/// <param name="extraDigitsMask">Indicates the mask that contains all extra digits used.</param>
public sealed partial class BivalueOddagonType3Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	ref readonly CellMap loopCells,
	Digit digit1,
	Digit digit2,
	[Property] ref readonly CellMap extraCells,
	[Property] Mask extraDigitsMask
) : BivalueOddagonStep(conclusions, views, options, in loopCells, digit1, digit2), IExtraCellListTrait
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | ExtraDigitsMask);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [LoopStr, Digit1Str, Digit2Str, DigitsStr, ExtraCellsStr]),
			new(SR.ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, ExtraCellsStr, DigitsStr])
		];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			.. base.Factors,
			Factor.Create(
				"Factor_BivalueOddagonSubsetSizeFactor",
				[nameof(IExtraCellListTrait.ExtraCellSize)],
				GetType(),
				static args => OeisSequences.A004526((int)args![0]!)
			)
		];

	/// <inheritdoc/>
	int IExtraCellListTrait.ExtraCellSize => ExtraCells.Count;

	private string Digit1Str => Options.Converter.DigitConverter((Mask)(1 << Digit1));

	private string Digit2Str => Options.Converter.DigitConverter((Mask)(1 << Digit2));

	private string DigitsStr => Options.Converter.DigitConverter(ExtraDigitsMask);

	private string ExtraCellsStr => Options.Converter.CellConverter(ExtraCells);
}
