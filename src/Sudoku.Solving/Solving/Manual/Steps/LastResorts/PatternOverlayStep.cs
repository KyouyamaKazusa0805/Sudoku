namespace Sudoku.Solving.Manual.Steps.LastResorts;

/// <summary>
/// Provides with a step that is a <b>Pattern Overlay</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
public sealed record PatternOverlayStep(
	ImmutableArray<Conclusion> Conclusions
) : LastResortStep(Conclusions, ImmutableArray.Create<PresentationData>())
{
	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit => Conclusions[0].Digit;

	/// <inheritdoc/>
	public override decimal Difficulty => 8.5M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.Pom;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.Pom;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.SingleDigitPatterns;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}
}
