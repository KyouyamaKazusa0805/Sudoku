using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Full House</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="house">The house to be displayed.</param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
public sealed partial class FullHouseStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] House house,
	Cell cell,
	Digit digit
) : SingleStep(conclusions, views, options, cell, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 1.0M;

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty => 100;

	/// <inheritdoc/>
	public override Technique Code => Technique.FullHouse;

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> [
			new(LocatingDifficultyFactorNames.Digit, 3 * Digit),
			new(
				LocatingDifficultyFactorNames.HouseType,
				27 * House.ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 }
			),
			new(LocatingDifficultyFactorNames.HousePosition, 9 * HotSpot.GetHotSpot(House))
		];
}
