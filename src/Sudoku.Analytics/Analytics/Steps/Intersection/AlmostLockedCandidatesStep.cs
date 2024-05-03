namespace Sudoku.Analytics.Steps;

public partial class AlmostLockedCandidatesStep
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 45;

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

	private string TargetCellsStr => Options.Converter.CellConverter(CoverCells);
}
