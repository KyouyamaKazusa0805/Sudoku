namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bowman's Bingo</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="contradictionLinks">Indicates the list of contradiction links.</param>
public sealed partial class BowmanBingoStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[RecordParameter] Conclusion[] contradictionLinks
) : LastResortStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.0M;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Length, StepRatingHelper.GetExtraDifficultyByLength(ContradictionLinks.Length))];

	/// <inheritdoc/>
	public override Technique Code => Technique.BowmanBingo;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [ContradictionSeriesStr]), new(ChineseLanguage, [ContradictionSeriesStr])];

	private unsafe string ContradictionSeriesStr
	{
		get
		{
			var snippets = new List<string>();
			foreach (var conclusion in ContradictionLinks)
			{
				snippets.Add(Options.Converter.ConclusionConverter([conclusion]));
			}

			return string.Join(" -> ", snippets);
		}
	}
}
