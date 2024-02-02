namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pivot">Indicates the pivot cell.</param>
/// <param name="branches">Indicates the branches.</param>
/// <param name="zDigitsMask">Indicates the digits mask as eliminations.</param>
public sealed partial class DeathBlossomStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryCosntructorParameter] Cell pivot,
	[PrimaryCosntructorParameter] NormalBlossomBranchCollection branches,
	[PrimaryCosntructorParameter] Mask zDigitsMask
) : DeathBlossomBaseStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.DeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PivotStr, BranchesStr]), new(ChineseLanguage, [PivotStr, BranchesStr])];

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Petals, A002024(Branches.Count) * .1M)];

	private string PivotStr => Options.Converter.CellConverter([Pivot]);

	private string BranchesStr
		=> string.Join(
			ResourceDictionary.Get("Comma", ResultCurrentCulture),
			[.. from branch in Branches select $"{Options.Converter.DigitConverter((Mask)(1 << branch.Key))} - {branch.AlsPattern}"]
		);
}
