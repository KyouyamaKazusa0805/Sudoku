namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Pattern Overlay</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
internal sealed record PatternOverlayStep(Conclusion[] Conclusions) : LastResortStep(Conclusions, null)
{
	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit => Conclusions[0].Digit;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.5M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.PatternOverlay;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.PatternOverlay;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.SingleDigitPattern;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitStr } }, { "zh", new[] { DigitStr } } };

	private string DigitStr => (Digit + 1).ToString();
}
