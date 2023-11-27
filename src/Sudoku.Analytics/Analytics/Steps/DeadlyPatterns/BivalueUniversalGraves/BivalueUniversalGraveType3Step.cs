using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Facts;
using Sudoku.Rendering;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="trueCandidates">Indicates the true candidates used.</param>
/// <param name="subsetDigitsMask">Indicates the mask of subset digits.</param>
/// <param name="cells">Indicates the subset cells used.</param>
/// <param name="emptyCellsCount">The number of empty cells.</param>
/// <param name="isNaked">Indicates whether the subset is naked.</param>
public sealed partial class BivalueUniversalGraveType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] scoped ref readonly CandidateMap trueCandidates,
	[Data] Mask subsetDigitsMask,
	[Data] scoped ref readonly CellMap cells,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int emptyCellsCount,
	[Data] bool isNaked
) : BivalueUniversalGraveStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType3;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Size, Size * .1M), new(ExtraDifficultyFactorNames.Hidden, IsNaked ? 0 : .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [TrueCandidatesStr, SubsetTypeStr, SizeStr, ExtraDigitsStr, CellsStr]),
			new(ChineseLanguage, [TrueCandidatesStr, SubsetTypeStr, SizeStr, CellsStr, ExtraDigitsStr])
		];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> [
			new(LocatingDifficultyFactorNames.EmptyCell, 560 * Math.Round(_emptyCellsCount / 11M, 2)),
			new(LocatingDifficultyFactorNames.TrueCandidate, Cells.Count * 60),
			new(LocatingDifficultyFactorNames.Size, Cells.Count * 9),
			new(LocatingDifficultyFactorNames.DigitVariance, GetDigitVariance(TrueCandidates) * 27)
		];

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => PopCount((uint)SubsetDigitsMask);

	private string TrueCandidatesStr => Options.Converter.CandidateConverter(TrueCandidates);

	private string SubsetTypeStr => GetString(IsNaked ? "NakedKeyword" : "HiddenKeyword")!;

	private string SizeStr => TechniqueFact.GetSubsetName(Size);

	private string ExtraDigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string CellsStr => Options.Converter.CellConverter(Cells);


	/// <summary>
	/// Try to get digit variance for true candidates.
	/// </summary>
	private int GetDigitVariance(scoped ref readonly CandidateMap trueCandidates)
	{
		var result = 0;
		foreach (var pair in trueCandidates.GetSubsets(2))
		{
			if (pair[0] % 9 != pair[1] % 9)
			{
				result++;
			}
		}

		return result;
	}
}
