using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Facts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="trueCandidates">Indicates the true candidates used.</param>
/// <param name="subsetDigitsMask">Indicates the mask of subset digits.</param>
/// <param name="cells">Indicates the subset cells used.</param>
/// <param name="isNaked">Indicates whether the subset is naked.</param>
public sealed partial class BivalueUniversalGraveType3Step(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] scoped ref readonly CandidateMap trueCandidates,
	[DataMember] Mask subsetDigitsMask,
	[DataMember] scoped ref readonly CellMap cells,
	[DataMember] bool isNaked
) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType3;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [new(ExtraDifficultyCaseNames.Size, Size * .1M), new(ExtraDifficultyCaseNames.Hidden, IsNaked ? 0 : .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [TrueCandidatesStr, SubsetTypeStr, SizeStr, ExtraDigitsStr, CellsStr]),
			new(ChineseLanguage, [TrueCandidatesStr, SubsetTypeStr, SizeStr, CellsStr, ExtraDigitsStr])
		];

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => PopCount((uint)SubsetDigitsMask);

	private string TrueCandidatesStr => TrueCandidates.ToString();

	private string SubsetTypeStr => GetString(IsNaked ? "NakedKeyword" : "HiddenKeyword")!;

	private string SizeStr => TechniqueFact.GetSubsetName(Size);

	private string ExtraDigitsStr => DigitNotation.ToString(SubsetDigitsMask);

	private string CellsStr => Cells.ToString();
}
