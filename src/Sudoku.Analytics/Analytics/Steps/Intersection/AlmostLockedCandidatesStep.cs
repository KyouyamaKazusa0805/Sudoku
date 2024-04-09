namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Candidates</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask that contains the digits used.</param>
/// <param name="baseCells">Indicates the base cells.</param>
/// <param name="targetCells">Indicates the target cells.</param>
/// <param name="hasValueCell">Indicates whether the step contains value cells.</param>
public sealed partial class AlmostLockedCandidatesStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] scoped ref readonly CellMap baseCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap targetCells,
	[PrimaryConstructorParameter] bool hasValueCell
) : IntersectionStep(conclusions, views, options), ISizeTrait
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 45;

	/// <inheritdoc/>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public override Technique Code
		=> (HasValueCell, Size) switch
		{
			(_, 2) => Technique.AlmostLockedPair,
			(true, 3) => Technique.AlmostLockedTripleValueType,
			(_, 3) => Technique.AlmostLockedTriple,
			(true, 4) => Technique.AlmostLockedQuadrupleValueType,
			(_, 4) => Technique.AlmostLockedQuadruple
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, BaseCellsStr, TargetCellsStr]), new(ChineseLanguage, [DigitsStr, BaseCellsStr, TargetCellsStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [new AlmostLockedCandidatesSizeFactor(), new AlmostLockedCandidatesValueCellExistenceFactor()];

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string BaseCellsStr => Options.Converter.CellConverter(BaseCells);

	private string TargetCellsStr => Options.Converter.CellConverter(TargetCells);
}
