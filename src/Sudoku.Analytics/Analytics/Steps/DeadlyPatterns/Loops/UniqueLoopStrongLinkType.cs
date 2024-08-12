namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Strong Link Type</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="loopPath"><inheritdoc/></param>
/// <param name="extraDigitsCellsCount">Indicates the number cells containing extra digits.</param>
/// <param name="conjugatePairs">Indicates the conjugate pairs.</param>
public sealed partial class UniqueLoopStrongLinkType(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap loop,
	Cell[] loopPath,
	[PrimaryConstructorParameter] int extraDigitsCellsCount,
	[PrimaryConstructorParameter] Conjugate[] conjugatePairs
) : UniqueLoopStep(conclusions, views, options, digit1, digit2, in loop, loopPath)
{
	/// <inheritdoc/>
	public override int Type => 5;

	/// <inheritdoc/>
	public override Technique Code => Technique.UniqueLoopStrongLinkType;

	/// <inheritdoc/>
	public override Interpolation[]? Interpolations
		=> [
			new(EnglishLanguage, [Digit1Str, Digit2Str, LoopStr, ConjugatePairsStr]),
			new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, ConjugatePairsStr])
		];

	private string ConjugatePairsStr
	{
		get
		{
			var culture = GetCulture(null);
			return string.Join(SR.Get("_Token_Comma", culture), from cp in ConjugatePairs select cp.ToString(culture));
		}
	}


	/// <inheritdoc/>
	public override string GetName(IFormatProvider? formatProvider)
	{
		var culture = GetCulture(null);
		var uniqueLoopName = SR.Get("UniqueNameName", culture);
		return $"{uniqueLoopName} + {extraDigitsCellsCount}";
	}
}
