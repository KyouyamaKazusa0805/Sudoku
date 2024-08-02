namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Law of Leftover (LoL)</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
/// <param name="set1House">Indicates the house of <see cref="Set1"/>.</param>
/// <param name="set2House">Indicates the house of <see cref="Set2"/>.</param>
/// <param name="set1">Indicates the first set to be used.</param>
/// <param name="set2">Indicates the second set to be used.</param>
public sealed partial class LawOfLeftoverStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] House set1House,
	[PrimaryConstructorParameter] House set2House,
	[PrimaryConstructorParameter] ref readonly CellMap set1,
	[PrimaryConstructorParameter] ref readonly CellMap set2
) : IntersectionStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 20;

	/// <inheritdoc/>
	public override Technique Code => Technique.LawOfLeftover;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [new(EnglishLanguage, [Set1Str, Set2Str]), new(ChineseLanguage, [Set1Str, Set2Str])];

	private string Set1Str => Options.Converter.CellConverter(Set1);

	private string Set2Str => Options.Converter.CellConverter(Set2);
}
