using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-Branch W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="leaves">The leaves of the pattern.</param>
/// <param name="root">The root cells that corresponds to each leaf.</param>
/// <param name="house">Indicates the house that all cells in <see cref="Root"/> lie in.</param>
public sealed partial class MultiBranchWWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] scoped ref readonly CellMap leaves,
	[Data] scoped ref readonly CellMap root,
	[Data] House house
) : IrregularWingStep(conclusions, views, options)
{
	/// <summary>
	/// Indicates the number of branches of the technique.
	/// </summary>
	public int Size => Leaves.Count;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override Technique Code => Technique.MultiBranchWWing;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Size, Size switch { 2 => 0, 3 => .3M, 4 => .6M, 5 => 1.0M, _ => 1.4M })];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [LeavesStr, RootStr, HouseStr]), new(ChineseLanguage, [RootStr, HouseStr, LeavesStr])];

	private string LeavesStr => Options.Converter.CellConverter(Leaves);

	private string RootStr => Options.Converter.CellConverter(Root);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
