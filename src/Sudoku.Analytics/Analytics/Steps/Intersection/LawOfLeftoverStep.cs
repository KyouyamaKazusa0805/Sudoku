namespace Sudoku.Analytics.Steps;

public partial class LawOfLeftoverStep
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 20;

	/// <inheritdoc/>
	public override Technique Code => Technique.LawOfLeftover;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [Set1Str, Set2Str]), new(ChineseLanguage, [Set1Str, Set2Str])];

	private string Set1Str => Options.Converter.CellConverter(Set1);

	private string Set2Str => Options.Converter.CellConverter(Set2);
}
