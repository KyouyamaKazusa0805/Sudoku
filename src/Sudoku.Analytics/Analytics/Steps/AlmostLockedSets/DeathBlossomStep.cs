using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts.ObjectModel;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;
using static System.Algorithm.Sequences;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pivot">Indicates the pivot cell.</param>
/// <param name="branches">Indicates the branches.</param>
/// <param name="endDigitsMask">Indicates the end-point digits mask.</param>
public sealed partial class DeathBlossomStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Cell pivot,
	[Data] BlossomBranch branches,
	[Data] Mask endDigitsMask
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.2M;

	/// <inheritdoc/>
	public override Technique Code => Technique.DeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PivotStr, BranchesStr]), new(ChineseLanguage, [PivotStr, BranchesStr])];

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Petals, A002024(Branches.Count) * .1M)];

	private string PivotStr => Options.Converter.CellConverter([Pivot]);

	private string BranchesStr
		=> string.Join(
			GetString("Comma"),
			[
				..
				from branch in Branches
				select $"{Options.Converter.DigitConverter((Mask)(1 << branch.Digit))} - {branch.AlsPattern}"
			]
		);
}
