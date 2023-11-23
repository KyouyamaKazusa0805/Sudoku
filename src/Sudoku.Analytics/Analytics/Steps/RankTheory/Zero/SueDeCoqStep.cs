using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Concepts.Converters;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Sue de Coq</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="block">Indicates the block index that the current pattern used.</param>
/// <param name="line">Indicates the line (row or column) index that the current pattern used.</param>
/// <param name="blockMask">Indicates the block mask.</param>
/// <param name="lineMask">Indicates the line mask.</param>
/// <param name="intersectionMask">Indicates the intersection mask.</param>
/// <param name="isCannibalistic">Indicates whether the SdC is cannibalistic.</param>
/// <param name="isolatedDigitsMask">The isolated digits mask.</param>
/// <param name="blockCells">Indicates the cells that the current pattern used in a block.</param>
/// <param name="lineCells">Indicates the cells that the current pattern used in a line (row or column).</param>
/// <param name="intersectionCells">
/// Indicates the cells that the current pattern used in an intersection of <see cref="BlockCells"/> and <see cref="LineCells"/>.
/// </param>
public sealed partial class SueDeCoqStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] House block,
	[Data] House line,
	[Data] Mask blockMask,
	[Data] Mask lineMask,
	[Data] Mask intersectionMask,
	[Data] bool isCannibalistic,
	[Data] Mask isolatedDigitsMask,
	[Data] scoped ref readonly CellMap blockCells,
	[Data] scoped ref readonly CellMap lineCells,
	[Data] scoped ref readonly CellMap intersectionCells
) : ZeroRankStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.0M;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsCannibalistic, IsolatedDigitsMask) switch
		{
			(true, _) => Technique.SueDeCoqCannibalism,
			(_, not 0) => Technique.SueDeCoqIsolated,
			_ => Technique.SueDeCoq
		};

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(ExtraDifficultyFactorNames.Isolated, IsolatedDigitsMask != 0 ? .1M : 0),
			new(ExtraDifficultyFactorNames.Cannibalism, IsCannibalistic ? .2M : 0)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [IntersectionCellsStr, IntersectionDigitsStr, BlockCellsStr, BlockDigitsStr, LineCellsStr, LineDigitsStr]),
			new(ChineseLanguage, [IntersectionCellsStr, IntersectionDigitsStr, BlockCellsStr, BlockDigitsStr, LineCellsStr, LineDigitsStr])
		];

	private string IntersectionCellsStr => Options.Converter.CellConverter(IntersectionCells);

	private string IntersectionDigitsStr
		=> new LiteralCoordinateConverter(DigitsSeparator: string.Empty).DigitConverter(IntersectionMask);

	private string BlockCellsStr => Options.Converter.CellConverter(BlockCells);

	private string BlockDigitsStr => new LiteralCoordinateConverter(DigitsSeparator: string.Empty).DigitConverter(BlockMask);

	private string LineCellsStr => Options.Converter.CellConverter(LineCells);

	private string LineDigitsStr => new LiteralCoordinateConverter(DigitsSeparator: string.Empty).DigitConverter(LineMask);
}
