using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts.ObjectModel;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

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
public partial class DeathBlossomStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Cell pivot,
	[Data] BlossomBranch branches,
	[Data] Mask endDigitsMask
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.3M;

	/// <inheritdoc/>
	public override Technique Code => Technique.DeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PivotStr, BranchesStr]), new(ChineseLanguage, [PivotStr, BranchesStr])];

	private string PivotStr => Options.Converter.CellConverter([Pivot]);

	private string BranchesStr
		=> string.Join(
			GetString("Comma"),
			from pair in Branches
			let digit = pair.Key
			let branch = pair.Value
			select $"{Options.Converter.DigitConverter((Mask)(1 << digit))} - {branch}"
		);
}
