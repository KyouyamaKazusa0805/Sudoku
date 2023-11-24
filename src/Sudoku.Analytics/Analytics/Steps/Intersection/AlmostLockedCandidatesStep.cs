using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Candidates</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask that contains the digits used.</param>
/// <param name="baseCells">Indicates the base cells.</param>
/// <param name="targetCells">Indicates the target cells.</param>
/// <param name="hasValueCell">Indicates whether the step contains value cells.</param>
public sealed partial class AlmostLockedCandidatesStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Mask digitsMask,
	[Data] scoped ref readonly CellMap baseCells,
	[Data] scoped ref readonly CellMap targetCells,
	[Data] bool hasValueCell
) : IntersectionStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the number of digits used.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public override Technique Code
		=> (HasValueCell, Size) switch
		{
			(_, 2) => Technique.AlmostLockedPair,
			(true, 3) => Technique.AlmostLockedTripleValueType,
			(_, 3) => Technique.AlmostLockedTriple,
			(true, 4) => Technique.AlmostLockedQuadrupleValueType,
			(_, 4) => Technique.AlmostLockedQuadruple
		};

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(ExtraDifficultyFactorNames.Size, Size switch { 2 => 0, 3 => .7M, 4 => 1.2M }),
			new(ExtraDifficultyFactorNames.ValueCell, HasValueCell ? Size switch { 2 or 3 => .1M, 4 => .2M } : 0)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, BaseCellsStr, TargetCellsStr]), new(ChineseLanguage, [DigitsStr, BaseCellsStr, TargetCellsStr])];

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string BaseCellsStr => Options.Converter.CellConverter(BaseCells);

	private string TargetCellsStr => Options.Converter.CellConverter(TargetCells);
}
