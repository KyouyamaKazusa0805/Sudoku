namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Anonymous Deadly Pattern Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="patternCandidates"><inheritdoc/></param>
/// <param name="targetCells">Indicates the target cells.</param>
/// <param name="subsetCells">Indicates the subset cells.</param>
/// <param name="subsetDigitsMask">Indicates the extra digits used.</param>
public sealed partial class AnonymousDeadlyPatternType3Step(
	ReadOnlyMemory<Conclusion> conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] ref readonly CandidateMap patternCandidates,
	[Property] ref readonly CellMap targetCells,
	[Property] ref readonly CellMap subsetCells,
	[Property] Mask subsetDigitsMask
) :
	AnonymousDeadlyPatternStep(conclusions, views, options, patternCandidates.Digits, patternCandidates.Cells),
	IPatternType3StepTrait<AnonymousDeadlyPatternType3Step>
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitsStr, ExtraCellsStr]),
			new(SR.ChineseLanguage, [DigitsStr, CellsStr, ExtraCellsStr, ExtraDigitsStr])
		];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_AnonymousDeadlyPatternSubsetSizeFactor",
				[nameof(IPatternType3StepTrait<AnonymousDeadlyPatternType3Step>.SubsetSize)],
				GetType(),
				static args => (int)args![0]!
			)
		];

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | SubsetDigitsMask);

	/// <inheritdoc/>
	bool IPatternType3StepTrait<AnonymousDeadlyPatternType3Step>.IsHidden => false;

	/// <inheritdoc/>
	int IPatternType3StepTrait<AnonymousDeadlyPatternType3Step>.SubsetSize => Mask.PopCount(SubsetDigitsMask);

	private string ExtraDigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string ExtraCellsStr => Options.Converter.CellConverter(SubsetCells);
}
