namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chain</b> or <b>Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern">Indicates the chain or loop pattern.</param>
public sealed partial class NormalChainStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] ChainPattern pattern
) : ChainStep(conclusions, views, options)
{
	/// <summary>
	/// Indicates the sort key that can be used as chaining comparison.
	/// </summary>
	public byte SortKey => Pattern.GetSortKey(Conclusions.AsSet());

	/// <inheritdoc/>
	public override int BaseDifficulty => Pattern is Loop ? 45 : 46;

	/// <summary>
	/// Indicates the chain length.
	/// </summary>
	public int ChainLength => Pattern.Length;

	/// <inheritdoc/>
	public override Technique Code => Pattern.GetTechnique(Conclusions.AsSet());

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [ChainString]), new(ChineseLanguage, [ChainString])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new ChainLengthFactor()];

	private string ChainString => Pattern.ToString("m", ResultCurrentCulture);
}
