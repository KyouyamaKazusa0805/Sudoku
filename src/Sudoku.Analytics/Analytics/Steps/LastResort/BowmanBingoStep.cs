namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bowman's Bingo</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="contradictionLinks">Indicates the list of contradiction links.</param>
public sealed partial class BowmanBingoStep(Conclusion[] conclusions, View[]? views, [DataMember] Conclusion[] contradictionLinks) :
	LastResortStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.0M;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [new(ExtraDifficultyCaseNames.Length, ChainDifficultyRating.GetExtraDifficultyByLength(ContradictionLinks.Length))];

	/// <inheritdoc/>
	public override Technique Code => Technique.BowmanBingo;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [ContradictionSeriesStr]), new(ChineseLanguage, [ContradictionSeriesStr])];

	private unsafe string ContradictionSeriesStr
	{
		get
		{
			static string elementToString(Conclusion conclusion) => conclusion.ToString();
			scoped var sb = new StringHandler();
			sb.AppendRangeWithSeparator(ContradictionLinks, &elementToString, " -> ");

			return sb.ToString();
		}
	}
}
