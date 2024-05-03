namespace Sudoku.Analytics.Steps;

public partial class DirectIntersectionStep
{
	/// <inheritdoc/>
	public override int BaseDifficulty
		=> BasedOn switch
		{
			Technique.FullHouse => 10,
			Technique.CrosshatchingBlock => 12,
			Technique.CrosshatchingRow or Technique.CrosshatchingColumn => 15,
#if false
			Technique.HiddenSingleBlock => 19,
			Technique.HiddenSingleRow or Technique.HiddenSingleColumn => 23,
#endif
			Technique.NakedSingle => 23,
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("TechiqueIsNotSupported"))
		} + 2;

	/// <inheritdoc/>
	public override Technique Code => BasedOn.ComplexSingleUsing(IsPointing ? Technique.Pointing : Technique.Claiming);

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, HouseStr, InterimCellStr, InterimDigitStr, TechniqueNameStr]),
			new(ChineseLanguage, [CellsStr, HouseStr, InterimCellStr, InterimDigitStr, TechniqueNameStr])
		];

	private string CellsStr => Options.Converter.CellConverter(IntersectionCells);

	private string HouseStr => Options.Converter.HouseConverter(1 << IntersectionHouse);

	private string InterimCellStr => Options.Converter.CellConverter(Interim);

	private string InterimDigitStr => Options.Converter.DigitConverter((Mask)(1 << InterimDigit));

	private string TechniqueNameStr => BasedOn.GetName(ResultCurrentCulture);
}
