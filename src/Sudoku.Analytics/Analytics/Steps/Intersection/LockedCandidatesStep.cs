namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Locked Candidates</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="baseSet">Indicates the house that the current locked candidates forms.</param>
/// <param name="coverSet">Indicates the house that the current locked candidates influences.</param>
public sealed partial class LockedCandidatesStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] House baseSet,
	[PrimaryConstructorParameter] House coverSet
) : IntersectionStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => BaseSet < 9 ? 2.6M : 2.8M;

	/// <inheritdoc/>
	public override Technique Code => BaseSet < 9 ? Technique.Pointing : Technique.Claiming;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, new[] { DigitStr, BaseSetStr, CoverSetStr } },
			{ ChineseLanguage, new[] { DigitStr, BaseSetStr, CoverSetStr } }
		};

	private string DigitStr => (Digit + 1).ToString();

	private string BaseSetStr => HouseFormatter.Format(1 << BaseSet);

	private string CoverSetStr => HouseFormatter.Format(1 << CoverSet);
}
