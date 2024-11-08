namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>(Grouped) Finned Chain</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="fins">Indicates the extra fins.</param>
/// <param name="basedComponent">Indicates the base component.</param>
public sealed partial class FinnedChainStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	NamedChain pattern,
	[Property] ref readonly CandidateMap fins,
	[Property] MultipleChainBasedComponent basedComponent
) : NormalChainStep(conclusions, views, options, pattern)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <summary>
	/// Indicates the base technique used.
	/// </summary>
	public Technique BasedOn => Casted.GetTechnique(Conclusions.AsSet());

	/// <inheritdoc/>
	public override Technique Code
		=> Casted.IsStrictlyGrouped || BasedComponent is not (MultipleChainBasedComponent.Cell or MultipleChainBasedComponent.House)
			? Technique.FinnedGroupedChain
			: Technique.FinnedChain;

	/// <inheritdoc/>
	public override Mask DigitsUsed => Casted.DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [ChainString, FinsStr]), new(SR.ChineseLanguage, [ChainString, FinsStr])];

	private string FinsStr => Fins.ToString(CoordinateConverter.GetInstance(Options.Converter));
}
