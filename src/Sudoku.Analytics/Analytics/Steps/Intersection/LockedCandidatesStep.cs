using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Locked Candidates</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="baseSet">Indicates the house that the current locked candidates forms.</param>
/// <param name="coverSet">Indicates the house that the current locked candidates influences.</param>
/// <param name="lockedCandidatesCellsCount">The number of cells of the locked candidates used.</param>
public sealed partial class LockedCandidatesStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Digit digit,
	[Data] House baseSet,
	[Data] House coverSet,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int lockedCandidatesCellsCount
) : IntersectionStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => BaseSet < 9 ? 2.6M : 2.8M;

	/// <inheritdoc/>
	public override Technique Code => BaseSet < 9 ? Technique.Pointing : Technique.Claiming;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, BaseSetStr, CoverSetStr]), new(ChineseLanguage, [DigitStr, BaseSetStr, CoverSetStr])];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> BaseSet switch
		{
			< 9 => [
				new(LocatingDifficultyFactorNames.HousePosition, HotSpot.GetHotSpot(BaseSet) * 9),
				new(LocatingDifficultyFactorNames.Digit, Digit * 3),
				new(LocatingDifficultyFactorNames.LockedCandidatesCellsCount, 4 - _lockedCandidatesCellsCount)
			],
			_ => [
				new(
					LocatingDifficultyFactorNames.HouseType,
					BaseSet.ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 } * 27
				),
				new(LocatingDifficultyFactorNames.HousePosition, HotSpot.GetHotSpot(BaseSet) * 9),
				new(LocatingDifficultyFactorNames.Digit, Digit * 3),
				new(LocatingDifficultyFactorNames.LockedCandidatesCellsCount, 4 - _lockedCandidatesCellsCount)
			]
		};

	/// <inheritdoc/>
	public override Formula LocatingDifficultyFormula
		=> new(a => Code == Technique.Pointing ? (a[0] + a[1]) * a[2] : (a[0] + a[1] + a[2]) * a[3]);

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string BaseSetStr => Options.Converter.HouseConverter(1 << BaseSet);

	private string CoverSetStr => Options.Converter.HouseConverter(1 << CoverSet);
}
