namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="house">Indicates the house where the current Hidden Single technique forms.</param>
/// <param name="enableAndIsLastDigit">
/// Indicates whether currently options enable "Last Digit" technique, and the current instance is a real Last Digit.
/// If the technique is not a Last Digit, the value must be <see langword="false"/>.
/// </param>
/// <param name="subtype"><inheritdoc/></param>
public partial class HiddenSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	[RecordParameter] House house,
	[RecordParameter] bool enableAndIsLastDigit,
	SingleSubtype subtype
) : SingleStep(conclusions, views, options, cell, digit, subtype)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> EnableAndIsLastDigit ? 1.1M : House < 9 ? Options.IsDirectMode ? 1.2M : 1.9M : Options.IsDirectMode ? 1.5M : 2.3M;

	/// <inheritdoc/>
	public override Technique Code
		=> (Options.IsDirectMode, EnableAndIsLastDigit) switch
		{
			(_, true) => Technique.LastDigit,
			(true, false) => (Technique)((int)Technique.CrosshatchingBlock + (int)House.ToHouseType()),
			_ => (Technique)((int)Technique.HiddenSingleBlock + (int)House.ToHouseType())
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, EnableAndIsLastDigit ? [DigitStr] : [HouseStr]),
			new(ChineseLanguage, EnableAndIsLastDigit ? [DigitStr] : [HouseStr])
		];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
