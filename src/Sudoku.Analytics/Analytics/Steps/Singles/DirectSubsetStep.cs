namespace Sudoku.Analytics.Steps;

public partial class DirectSubsetStep
{
	/// <summary>
	/// Indicates whether the used subset is a naked subset.
	/// </summary>
	public bool IsNaked
		=> SubsetTechnique is Technique.NakedPair or Technique.NakedPairPlus or Technique.LockedPair
		or Technique.NakedTriple or Technique.NakedTriplePlus or Technique.LockedTriple
		or Technique.NakedQuadruple or Technique.NakedQuadruplePlus;

	/// <summary>
	/// <inheritdoc
	///     cref="NakedSubsetStep(Conclusion[], View[], StepSearcherOptions, int, ref readonly CellMap, short, bool?)"
	///     path="/param[@name='isLocked']"/>
	/// </summary>
	public bool? IsLocked
		=> SubsetTechnique switch
		{
			Technique.NakedPair or Technique.NakedTriple or Technique.NakedQuadruple => null,
			Technique.NakedPairPlus or Technique.NakedTriplePlus or Technique.NakedQuadruplePlus => false,
			Technique.LockedPair or Technique.LockedTriple => true,
			_ => null
		};

	/// <inheritdoc/>
	public int Size => SubsetCells.Count;

	/// <inheritdoc/>
	public override int BaseDifficulty => IsNaked ? 33 : 37;

	/// <inheritdoc/>
	public override Technique Code => BasedOn.ComplexSingleUsing(SubsetTechnique);

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, HouseStr, InterimCellStr, InterimDigitStr, TechniqueNameStr, DigitsStr, SubsetNameStr]),
			new(ChineseLanguage, [CellsStr, HouseStr, InterimCellStr, InterimDigitStr, TechniqueNameStr, DigitsStr, SubsetNameStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new DirectSubsetSizeFactor(), new DirectSubsetIsLockedFactor()];

	/// <inheritdoc/>
	int ICellListTrait.CellSize => SubsetCells.Count;

	private string CellsStr => Options.Converter.CellConverter(SubsetCells);

	private string HouseStr => Options.Converter.HouseConverter(1 << SubsetHouse);

	private string InterimCellStr => Options.Converter.CellConverter(Interim);

	private string InterimDigitStr => Options.Converter.DigitConverter(InterimDigitsMask);

	private string TechniqueNameStr => BasedOn.GetName(ResultCurrentCulture);

	private string DigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string SubsetNameStr => SubsetTechnique.GetName(ResultCurrentCulture);
}
