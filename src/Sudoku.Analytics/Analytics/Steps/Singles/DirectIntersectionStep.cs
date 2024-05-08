namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Direct Intersection</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
/// <param name="cell"><inheritdoc cref="SingleStep.Cell" path="/summary"/></param>
/// <param name="digit"><inheritdoc cref="SingleStep.Digit" path="/summary"/></param>
/// <param name="intersectionCells">Indicates the intersection cells.</param>
/// <param name="intersectionHouse">Indicates the intersection house.</param>
/// <param name="interim">Indicates the interim cells.</param>
/// <param name="interimDigit">Indicates the interim digit.</param>
/// <param name="subtype"><inheritdoc cref="SingleStep.Subtype" path="/summary"/></param>
/// <param name="basedOn"><inheritdoc cref="ComplexSingleBaseStep.BasedOn" path="/summary"/></param>
/// <param name="isPointing">Indicates whether the current locked candidates pattern used is pointing.</param>
public sealed partial class DirectIntersectionStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	[PrimaryConstructorParameter] ref readonly CellMap intersectionCells,
	[PrimaryConstructorParameter] House intersectionHouse,
	[PrimaryConstructorParameter] ref readonly CellMap interim,
	[PrimaryConstructorParameter] Digit interimDigit,
	SingleSubtype subtype,
	Technique basedOn,
	[PrimaryConstructorParameter] bool isPointing
) : ComplexSingleBaseStep(
	conclusions,
	views,
	options,
	cell,
	digit,
	subtype,
	basedOn,
	[[isPointing ? Technique.Pointing : Technique.Claiming]]
)
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
