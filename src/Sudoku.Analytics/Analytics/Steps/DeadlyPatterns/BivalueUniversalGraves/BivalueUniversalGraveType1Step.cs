using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="emptyCellsCount">The number of empty cells.</param>
public sealed partial class BivalueUniversalGraveType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int emptyCellsCount
) : BivalueUniversalGraveStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType1;

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> [new(LocatingDifficultyFactorNames.EmptyCell, 560 * Math.Round(_emptyCellsCount / 11M, 2))];
}
