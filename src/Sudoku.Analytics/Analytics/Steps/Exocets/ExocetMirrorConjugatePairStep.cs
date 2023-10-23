using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Exocet (Mirror Conjugate Pair)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="endoTargetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="conjugatePairs">Indicates the conjugate pairs used.</param>
public sealed partial class ExocetMirrorConjugatePairStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Mask digitsMask,
	scoped ref readonly CellMap baseCells,
	scoped ref readonly CellMap targetCells,
	scoped ref readonly CellMap endoTargetCells,
	scoped ref readonly CellMap crosslineCells,
	[DataMember] Conjugate[] conjugatePairs
) : ExocetStep(
	conclusions,
	views,
	options,
	digitsMask,
	in baseCells,
	in targetCells,
	in endoTargetCells,
	in crosslineCells
)
{
	/// <inheritdoc/>
	public override Technique Code => Delta < 0 ? Technique.SeniorExocetMirror : Technique.JuniorExocetMirror;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Mirror, .1M)];
}
