using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

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
/// <param name="enableAndIsLastDigit">
/// Indicates whether currently options enable "Last Digit" technique, and the current instance is a real Last Digit.
/// </param>
public sealed partial class HiddenSingleStep(
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
	public override decimal BaseDifficulty => this switch { { EnableAndIsLastDigit: true } => 1.1M, { House: < 9 } => 1.2M, _ => 1.5M };

	/// <inheritdoc/>
	public override TechniqueFormat Format => $"{(EnableAndIsLastDigit ? "LastDigit" : "HiddenSingle")}";

	/// <inheritdoc/>
	public override Technique Code
		=> EnableAndIsLastDigit ? Technique.LastDigit : (Technique)((int)Technique.HiddenSingleBlock + (int)House.ToHouseType());

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, EnableAndIsLastDigit ? [DigitStr] : [HouseStr]),
			new(ChineseLanguage, EnableAndIsLastDigit ? [DigitStr] : [HouseStr])
		];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
