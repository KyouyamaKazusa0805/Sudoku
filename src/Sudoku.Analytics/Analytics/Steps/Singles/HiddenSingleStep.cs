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
public partial class HiddenSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	[Data] House house,
	[Data] bool enableAndIsLastDigit
) : SingleStep(conclusions, views, options, cell, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> EnableAndIsLastDigit ? 1.1M : House < 9 ? Options.IsDirectMode ? 1.2M : 2.2M : Options.IsDirectMode ? 1.5M : 2.3M;

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty
		=> Code switch { Technique.LastDigit => 200, Technique.HiddenSingleBlock or Technique.CrosshatchingBlock => 250, _ => 300 };

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


	private static int ExcluderValueSelector((int Left, House Right) p)
		=> p.Right.ToHouseType() switch { HouseType.Block => 3, HouseType.Row => 1, HouseType.Column => 2 } + p.Left * 3;
}
