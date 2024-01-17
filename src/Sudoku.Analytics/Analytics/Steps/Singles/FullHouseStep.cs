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
	[RecordParameter] House house,
	Cell cell,
	Digit digit
) : SingleStep(
	conclusions,
	views,
	options,
	cell,
	digit,
	house switch
	{
		< 9 => SingleSubtype.FullHouseBlock,
		>= 9 and < 18 => SingleSubtype.FullHouseRow,
		_ => SingleSubtype.FullHouseColumn
	}
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 1.0M;

	/// <inheritdoc/>
	public override Technique Code => Technique.FullHouse;
}
