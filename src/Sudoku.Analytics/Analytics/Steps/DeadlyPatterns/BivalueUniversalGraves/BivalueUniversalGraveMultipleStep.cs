namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave + n</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="trueCandidates">Indicates the true candidates.</param>
public sealed partial class BivalueUniversalGraveMultipleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] scoped ref readonly CandidateMap trueCandidates
) : BivalueUniversalGraveStep(conclusions, views, options), ITrueCandidatesTrait
{
	/// <summary>
	/// <inheritdoc cref="Step.EnglishName" path="/summary"/>
	/// </summary>
	public override string EnglishName => $"{base.EnglishName[..^4]} + {TrueCandidates.Count}";

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGravePlusN;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CandidatesStr]), new(ChineseLanguage, [CandidatesStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new BivalueUniversalGraveMultipleTrueCandidateFactor(Options)];

	private string CandidatesStr => Options.Converter.CandidateConverter(TrueCandidates);


	/// <inheritdoce/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string GetName(CultureInfo? culture = null)
		=> $"{base.GetName(culture ?? ResultCurrentCulture)[..^4]} + {TrueCandidates.Count}";
}
