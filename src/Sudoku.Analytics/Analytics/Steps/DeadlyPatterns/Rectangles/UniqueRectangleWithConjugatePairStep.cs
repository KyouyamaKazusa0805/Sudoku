namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Conjugate Pair(s)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="code"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="conjugatePairs">Indicates the conjugate pairs used.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public partial class UniqueRectangleWithConjugatePairStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Technique code,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	bool isAvoidable,
	[PrimaryConstructorParameter] Conjugate[] conjugatePairs,
	int absoluteOffset
) :
	UniqueRectangleStep(conclusions, views, options, code, digit1, digit2, in cells, isAvoidable, absoluteOffset),
	IConjugatePairTrait
{
	/// <inheritdoc/>
	public sealed override int BaseDifficulty => base.BaseDifficulty - 1;

	/// <inheritdoc/>
	public override int Type
		=> Code switch { Technique.UniqueRectangleType4 => 4, Technique.UniqueRectangleType6 => 6, _ => base.Type };

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, Prefix, Suffix, ConjPairsStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, ConjPairsStr])
		];

	/// <inheritdoc/>
	public sealed override FactorCollection Factors
		=> [new RectangleConjugatePairsCountFactor(), new RectangleIsAvoidableFactor()];

	/// <inheritdoc/>
	int IConjugatePairTrait.ConjugatePairsCount => ConjugatePairs.Length;

	private string ConjPairsStr => Options.Converter.ConjugateConverter(ConjugatePairs);

	private string Prefix => ConjugatePairs.Length == 1 ? "a " : string.Empty;

	private string Suffix => ConjugatePairs.Length == 1 ? string.Empty : "s";
}
