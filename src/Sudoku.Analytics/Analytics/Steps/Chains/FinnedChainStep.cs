namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Finned (Grouped) Chain</b> or <b>Finned (Grouped) Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="fins">Indicates the extra fins.</param>
public sealed partial class FinnedChainStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	NamedChain pattern,
	[PrimaryConstructorParameter] ref readonly CandidateMap fins
) : NormalChainStep(conclusions, views, options, pattern)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <summary>
	/// Indicates the base technique used.
	/// </summary>
	public Technique BasedOn => Pattern.GetTechnique(Conclusions.AsSet());

	/// <inheritdoc/>
	public override Technique Code => IsGrouped ? Technique.FinnedGroupedChain : Technique.FinnedChain;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [new(SR.EnglishLanguage, [ChainString, FinsStr]), new(SR.ChineseLanguage, [ChainString, FinsStr])];

	private string FinsStr => Fins.ToString(CoordinateConverter.GetInstance(Options.Converter));
}
