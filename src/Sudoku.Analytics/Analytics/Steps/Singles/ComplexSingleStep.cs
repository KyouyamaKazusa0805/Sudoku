namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="subtype"><inheritdoc/></param>
/// <param name="basedOn"><inheritdoc/></param>
/// <param name="indirectTechniques"><inheritdoc/></param>
public sealed partial class ComplexSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	SingleSubtype subtype,
	Technique basedOn,
	Technique[][] indirectTechniques
) : ComplexSingleBaseStep(conclusions, views, options, cell, digit, subtype, basedOn, indirectTechniques)
{
	/// <inheritdoc/>
	public override int BaseDifficulty
		=> BasedOn switch
		{
			Technique.FullHouse => 10,
			Technique.CrosshatchingBlock => 12,
			Technique.CrosshatchingRow => 15,
			Technique.CrosshatchingColumn => 15,
			Technique.NakedSingle => 23
		} + 5;

	/// <inheritdoc/>
	public override Technique Code
		=> BasedOn switch
		{
			Technique.FullHouse => Technique.ComplexFullHouse,
			Technique.CrosshatchingBlock => Technique.ComplexCrosshatchingBlock,
			Technique.CrosshatchingRow => Technique.ComplexCrosshatchingRow,
			Technique.CrosshatchingColumn => Technique.ComplexCrosshatchingColumn,
			Technique.NakedSingle => Technique.ComplexNakedSingle
		};
}
