namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="code"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="branches">Indicates the branches used.</param>
/// <param name="petals">Indicates the petals used.</param>
/// <param name="extraDigitsMask">Indicates the mask that contains all extra digits.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleWithWingStep(
	Conclusion[] conclusions,
	View[]? views,
	Technique code,
	Digit digit1,
	Digit digit2,
	scoped in CellMap cells,
	bool isAvoidable,
	[PrimaryConstructorParameter] scoped in CellMap branches,
	[PrimaryConstructorParameter] scoped in CellMap petals,
	[PrimaryConstructorParameter] Mask extraDigitsMask,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	code,
	digit1,
	digit2,
	cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [
			(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			(
				ExtraDifficultyCaseNames.WingSize,
				Code switch
				{
					Technique.UniqueRectangleXyWing or Technique.AvoidableRectangleXyWing => .2M,
					Technique.UniqueRectangleXyzWing or Technique.AvoidableRectangleXyzWing => .3M,
					Technique.UniqueRectangleWxyzWing or Technique.AvoidableRectangleWxyzWing => .5M
				}
			)
		];

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, [D1Str, D2Str, CellsStr, BranchesStr, DigitsStr] },
			{ ChineseLanguage, [D1Str, D2Str, CellsStr, BranchesStr, DigitsStr] }
		};

	private string BranchesStr => Branches.ToString();

	private string DigitsStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);
}
