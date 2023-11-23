using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Locked Type</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="is2LinesWith2Cells"><inheritdoc/></param>
/// <param name="houses"><inheritdoc/></param>
/// <param name="corner1"><inheritdoc/></param>
/// <param name="corner2"><inheritdoc/></param>
/// <param name="candidates">Indicates the candidates used as locked one.</param>
public sealed partial class QiuDeadlyPatternLockedTypeStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	bool is2LinesWith2Cells,
	HouseMask houses,
	Cell? corner1,
	Cell? corner2,
	[Data(NamingRule = ">@Locked")] scoped ref readonly CandidateMap candidates
) : QiuDeadlyPatternStep(conclusions, views, options, is2LinesWith2Cells, houses, corner1, corner2)
{
	/// <inheritdoc/>
	public override int Type => 5;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.LockedDigit, .2M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [PatternStr, Quantifier, Number, SingularOrPlural, CandidateStr, BeVerb]),
			new(ChineseLanguage, [Number, PatternStr])
		];

	private string CandidateStr => Options.Converter.CandidateConverter(CandidatesLocked);

	private string Quantifier => CandidatesLocked.Count switch { 1 => string.Empty, 2 => " both", _ => " all" };

	private string Number => CandidatesLocked.Count == 1 ? " the" : $" {CandidatesLocked.Count}";

	private string SingularOrPlural => CandidatesLocked.Count == 1 ? "candidate" : "candidates";

	private string BeVerb => CandidatesLocked.Count == 1 ? "is" : "are";
}
