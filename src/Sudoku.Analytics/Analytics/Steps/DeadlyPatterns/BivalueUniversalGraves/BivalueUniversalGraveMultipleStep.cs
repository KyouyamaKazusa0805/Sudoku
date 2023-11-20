using System.Algorithm;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

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
	[Data] scoped ref readonly CandidateMap trueCandidates
) : BivalueUniversalGraveStep(conclusions, views, options)
{
	/// <summary>
	/// <inheritdoc cref="Step.Name" path="/summary"/>
	/// </summary>
	/// <remarks>
	/// <para><inheritdoc cref="Step.Name" path="//remarks"/></para>
	/// <para>
	/// In addition, the expression <c>..^4</c> means
	/// we have cut the name part " + n" (4 characters in total) in the full name "Bivalue Universal Grave + n".
	/// </para>
	/// </remarks>
	public override string Name => $"{base.Name[..^4]} + {TrueCandidates.Count}";

	/// <summary>
	/// <inheritdoc cref="Step.EnglishName" path="/summary"/>
	/// </summary>
	/// <remarks>
	/// <para><inheritdoc cref="Step.EnglishName" path="//remarks"/></para>
	/// <para><inheritdoc cref="Name" path="//remarks/para[2]"/></para>
	/// </remarks>
	public override string EnglishName => $"{base.EnglishName[..^4]} + {TrueCandidates.Count}";

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGravePlusN;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [new(ExtraDifficultyCaseNames.Size, Sequences.A002024(TrueCandidates.Count) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CandidatesStr]), new(ChineseLanguage, [CandidatesStr])];

	private string CandidatesStr => Options.Converter.CandidateConverter(TrueCandidates);
}
