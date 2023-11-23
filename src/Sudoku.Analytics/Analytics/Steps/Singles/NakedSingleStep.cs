using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="excluderHouses">Indicates the excluder houses.</param>
public sealed partial class NakedSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] House[] excluderHouses
) : SingleStep(conclusions, views, options, cell, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 2.3M;

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty => 162;

	/// <inheritdoc/>
	public override Technique Code => Technique.NakedSingle;

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> [new(LocatingDifficultyFactorNames.NakedSingleExcluder, _excluderHouses.Sum(ExcluderValue))];


	private int ExcluderValue(House house) => 100 * house.ToHouseType() switch { HouseType.Block => 3, HouseType.Row => 1, HouseType.Column => 2 };
}
