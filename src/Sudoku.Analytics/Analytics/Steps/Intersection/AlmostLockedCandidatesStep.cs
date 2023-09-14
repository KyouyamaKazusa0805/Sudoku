using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Candidates</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask that contains the digits used.</param>
/// <param name="baseCells">Indicates the base cells.</param>
/// <param name="targetCells">Indicates the target cells.</param>
/// <param name="hasValueCell">Indicates whether the step contains value cells.</param>
public sealed partial class AlmostLockedCandidatesStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] Mask digitsMask,
	[DataMember] scoped in CellMap baseCells,
	[DataMember] scoped in CellMap targetCells,
	[DataMember] bool hasValueCell
) : IntersectionStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the number of digits used.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public override Technique Code => (Technique)((int)Technique.AlmostLockedPair + Size - 2);

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [
			new(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .7M, 4 => 1.2M }),
			new(ExtraDifficultyCaseNames.ValueCell, HasValueCell ? Size switch { 2 or 3 => .1M, 4 => .2M } : 0)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, BaseCellsStr, TargetCellsStr]), new(ChineseLanguage, [DigitsStr, BaseCellsStr, TargetCellsStr])];

	private string DigitsStr => DigitNotation.ToString(DigitsMask);

	private string BaseCellsStr => BaseCells.ToString();

	private string TargetCellsStr => TargetCells.ToString();
}
