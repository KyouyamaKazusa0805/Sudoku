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
	StepGathererOptions options,
	[PrimaryConstructorParameter] ref readonly CellMap cells,
	[PrimaryConstructorParameter] bool isComplex,
	[PrimaryConstructorParameter] Mask digitsMask
) : ChainStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsMultiple => false;

	/// <inheritdoc/>
	public override bool IsDynamic => Code == Technique.ComplexRemotePair;

	/// <inheritdoc/>
	public override int Complexity => Cells.Count;

	/// <inheritdoc/>
	public override int BaseDifficulty => IsComplex ? 50 : 52;

	/// <inheritdoc/>
	public override Technique Code => IsComplex ? Technique.ComplexRemotePair : Technique.RemotePair;

	/// <inheritdoc/>
	public override Mask DigitsUsed => DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [CellsStr, FirstUnknownCharacterString, SecondUnknownCharacterString]),
			new(SR.ChineseLanguage, [CellsStr, FirstUnknownCharacterString, SecondUnknownCharacterString])
		];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string FirstUnknownCharacterString => UnknownCharacters[0].ToString();

	private string SecondUnknownCharacterString => UnknownCharacters[1].ToString();

	private ReadOnlyCharSequence UnknownCharacters => Options.BabaGroupInitialLetter.GetSequence(Options.BabaGroupLetterCasing);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is RemotePairStep comparer && Cells == comparer.Cells && IsComplex == comparer.IsComplex;
}
