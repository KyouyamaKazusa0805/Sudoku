namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Law of Leftover</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="set1House">Indicates the house of <see cref="Set1"/>.</param>
/// <param name="set2House">Indicates the house of <see cref="Set2"/>.</param>
/// <param name="set1">Indicates the first set.</param>
/// <param name="set2">Indicates the second set.</param>
public sealed partial class LawOfLeftoverStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] House set1House,
	[PrimaryConstructorParameter] House set2House,
	[PrimaryConstructorParameter] scoped ref readonly CellMap set1,
	[PrimaryConstructorParameter] scoped ref readonly CellMap set2
) : IntersectionStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 2.0M;

	/// <inheritdoc/>
	public override Technique Code => Technique.LawOfLeftover;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [Set1Str, Set2Str]), new(ChineseLanguage, [Set1Str, Set2Str])];

	private string Set1Str => Options.Converter.CellConverter(Set1);

	private string Set2Str => Options.Converter.CellConverter(Set2);
}
