namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Locked Candidates</b> technique.
/// </summary>
public sealed class LockedCandidatesStep(Conclusion[] conclusions, View[]? views, int digit, int baseSet, int coverSet) :
	IntersectionStep(conclusions, views)
{
	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; } = digit;

	/// <summary>
	/// Indicates the house that the current locked candidates forms.
	/// </summary>
	public int BaseSet { get; } = baseSet;

	/// <summary>
	/// Indicates the house that the current locked candidates influences.
	/// </summary>
	public int CoverSet { get; } = coverSet;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => BaseSet < 9 ? 2.6M : 2.8M;

	/// <inheritdoc/>
	public override Technique Code => BaseSet < 9 ? Technique.Pointing : Technique.Claiming;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.LockedCandidates;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, BaseSetStr, CoverSetStr } },
			{ "zh", new[] { DigitStr, BaseSetStr, CoverSetStr } }
		};

	private string DigitStr => (Digit + 1).ToString();

	private string BaseSetStr => HouseFormatter.Format(1 << BaseSet);

	private string CoverSetStr => HouseFormatter.Format(1 << CoverSet);
}
