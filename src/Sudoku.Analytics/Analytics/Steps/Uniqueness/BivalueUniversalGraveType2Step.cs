namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit">Indicates the extra digit.</param>
/// <param name="cells">Indicates the cells used.</param>
public sealed partial class BivalueUniversalGraveType2Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property(NamingRule = "Extra>@")] Digit digit,
	[Property] in CellMap cells
) : BivalueUniversalGraveStep(conclusions, views, options), ITrueCandidatesTrait, ICellListTrait
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType2;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(1 << ExtraDigit);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [ExtraDigitStr, CellsStr]), new(SR.ChineseLanguage, [CellsStr, ExtraDigitStr])];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_BivalueUniversalGraveType2TrueCandidateFactor",
				[nameof(ICellListTrait.CellSize)],
				GetType(),
				static args => OeisSequences.A002024((int)args![0]!)
			)
		];

	/// <inheritdoc/>
	int ICellListTrait.CellSize => Cells.Count;

	/// <inheritdoc/>
	CandidateMap ITrueCandidatesTrait.TrueCandidates => Cells * ExtraDigit;

	private string ExtraDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));

	private string CellsStr => Options.Converter.CellConverter(Cells);
}
