using System.Algorithm;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static System.Numerics.BitOperations;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Junior Exocet (Mirror Almost Hidden Set)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="extraCells">Indicates the cells that provides with the AHS rule.</param>
/// <param name="extraDigitsMask">Indicates the mask that holds the digits used by the AHS.</param>
public sealed partial class JuniorExocetMirrorAlmostHiddenSetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Mask digitsMask,
	scoped ref readonly CellMap baseCells,
	scoped ref readonly CellMap targetCells,
	scoped ref readonly CellMap crosslineCells,
	[DataMember] scoped ref readonly CellMap extraCells,
	[DataMember] Mask extraDigitsMask
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.JuniorExocetMirrorAlmostHiddenSet;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[]? ExtraDifficultyCases
		=> [
			new(ExtraDifficultyCaseNames.AlmostHiddenSet, .2M),
			new(ExtraDifficultyCaseNames.Size, Sequences.A002024(PopCount((uint)ExtraDigitsMask)) * .1M)
		];
}
