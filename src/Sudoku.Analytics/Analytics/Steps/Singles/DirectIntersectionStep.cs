namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Direct Intersection</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="intersectionCells">Indicates the intersection cells.</param>
/// <param name="intersectionHouse">Indicates the intersection house.</param>
/// <param name="interim">Indicates the interim cells.</param>
/// <param name="interimDigit">Indicates the interim digit.</param>
/// <param name="subtype"><inheritdoc/></param>
/// <param name="basedOn"><inheritdoc/></param>
/// <param name="isPointing">Indicates whether the current locked candidates pattern used is pointing.</param>
public sealed partial class DirectIntersectionStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	[PrimaryConstructorParameter] scoped ref readonly CellMap intersectionCells,
	[PrimaryConstructorParameter] House intersectionHouse,
	[PrimaryConstructorParameter] scoped ref readonly CellMap interim,
	[PrimaryConstructorParameter] Digit interimDigit,
	SingleTechniqueSubtype subtype,
	Technique basedOn,
	[PrimaryConstructorParameter] bool isPointing
) : ComplexSingleStep(
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
	/// <summary>
	/// Indicates the "Not supported" message.
	/// </summary>
	private const string NotSupportedMessage = "This technique usage doesn't use this property.";


	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> BasedOn switch
		{
			Technique.FullHouse => 1.0M,
			Technique.CrosshatchingBlock => 1.2M,
			Technique.CrosshatchingRow or Technique.CrosshatchingColumn => 1.5M,
#if false
			Technique.HiddenSingleBlock => 1.9M,
			Technique.HiddenSingleRow or Technique.HiddenSingleColumn => 2.3M,
#endif
			Technique.NakedSingle => 2.3M,
			_ => throw new NotSupportedException(TechniqueNotSupportedMessage)
		} + .2M;

	/// <inheritdoc/>
	public override Technique Code => BasedOn.ComplexSingleUsing(IsPointing ? Technique.Pointing : Technique.Claiming);

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, HouseStr, InterimCellStr, InterimDigitStr, TechniqueNameStr]),
			new(ChineseLanguage, [CellsStr, HouseStr, InterimCellStr, InterimDigitStr, TechniqueNameStr])
		];

	/// <inheritdoc/>
	protected override string PrefixName
	{
		[DoesNotReturn]
		get => throw new NotSupportedException(NotSupportedMessage);
	}

	/// <inheritdoc/>
	protected override int PrefixNameLength
	{
		[DoesNotReturn]
		get => throw new NotImplementedException(NotSupportedMessage);
	}

	private string CellsStr => Options.Converter.CellConverter(IntersectionCells);

	private string HouseStr => Options.Converter.HouseConverter(1 << IntersectionHouse);

	private string InterimCellStr => Options.Converter.CellConverter(Interim);

	private string InterimDigitStr => Options.Converter.DigitConverter((Mask)(1 << InterimDigit));

	private string TechniqueNameStr => BasedOn.GetName(ResultCurrentCulture);
}
