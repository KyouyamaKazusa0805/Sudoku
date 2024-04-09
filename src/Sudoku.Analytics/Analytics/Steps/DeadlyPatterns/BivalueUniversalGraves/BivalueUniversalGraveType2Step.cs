namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit">Indicates the extra digit.</param>
/// <param name="cells">Indicates the cells used.</param>
public sealed partial class BivalueUniversalGraveType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter(GeneratedMemberName = "ExtraDigit")] Digit digit,
	[PrimaryConstructorParameter] scoped ref readonly CellMap cells
) : BivalueUniversalGraveStep(conclusions, views, options), ITrueCandidatesTrait
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType2;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [ExtraDigitStr, CellsStr]), new(ChineseLanguage, [CellsStr, ExtraDigitStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new BivalueUniversalGraveType2TrueCandidateFactor()];

	/// <inheritdoc/>
	CandidateMap ITrueCandidatesTrait.TrueCandidates => Cells * ExtraDigit;

	private string ExtraDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));

	private string CellsStr => Options.Converter.CellConverter(Cells);
}
