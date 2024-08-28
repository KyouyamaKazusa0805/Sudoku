namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>(Complex) Remote Pair</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="isComplex">Indicates whether the pattern is complex.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
public sealed partial class RemotePairStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] ref readonly CellMap cells,
	[PrimaryConstructorParameter] bool isComplex,
	[PrimaryConstructorParameter] Mask digitsMask
) : BabaGroupingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => IsComplex ? 50 : 52;

	/// <inheritdoc/>
	public override Technique Code => IsComplex ? Technique.ComplexRemotePair : Technique.RemotePair;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [
			new(SR.EnglishLanguage, [CellsStr, FirstLetterStr, SecondLetterStr]),
			new(SR.ChineseLanguage, [CellsStr, FirstLetterStr, SecondLetterStr])
		];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string FirstLetterStr => "a";

	private string SecondLetterStr => "b";


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is RemotePairStep comparer && Cells == comparer.Cells && IsComplex == comparer.IsComplex;
}
