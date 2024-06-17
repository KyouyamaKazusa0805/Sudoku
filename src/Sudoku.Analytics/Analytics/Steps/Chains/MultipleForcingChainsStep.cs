namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Multiple Forcing Chains</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern">The pattern to be used.</param>
public sealed partial class MultipleForcingChainsStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] MultipleForcingChains pattern
) : ChainStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsMultiple => true;

	/// <inheritdoc/>
	public override bool IsDynamic => false;

	/// <summary>
	/// Indicates whether the pattern uses grouped nodes.
	/// </summary>
	public bool IsGrouped => Pattern.Exists(static chain => chain.IsGrouped);

	/// <inheritdoc/>
	public override int BaseDifficulty => 70;

	/// <inheritdoc/>
	public override int Complexity => Pattern.Complexity;

	/// <inheritdoc/>
	public override Technique Code => Pattern.IsCellMultiple ? Technique.CellForcingChains : Technique.RegionForcingChains;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [ChainsStr]), new(ChineseLanguage, [ChainsStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [
			new MultipleForcingChainsGroupedFactor(),
			new MultipleForcingChainsGroupedNodeFactor(),
			new MultipleForcingChainsLengthFactor()
		];

	private string ChainsStr => Pattern.ToString("m", Options.Converter ?? CoordinateConverter.GetConverter(ResultCurrentCulture));


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is MultipleForcingChainsStep comparer && Pattern.Equals(comparer.Pattern);

	/// <inheritdoc/>
	public override int CompareTo(Step? other)
		=> other is MultipleForcingChainsStep comparer ? Pattern.CompareTo(comparer.Pattern) : -1;
}
