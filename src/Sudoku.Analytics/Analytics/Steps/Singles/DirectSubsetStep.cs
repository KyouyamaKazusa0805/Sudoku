namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Direct Subset</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="subsetCells">Indicates the subset cells used.</param>
/// <param name="subsetDigitsMask">Indicates the digits that the subset used.</param>
/// <param name="subsetHouse">Indicates the subset house.</param>
/// <param name="interim">Indicates the interim.</param>
/// <param name="interimDigitsMask">Indicates the digits produced in interim.</param>
/// <param name="subtype"><inheritdoc/></param>
/// <param name="basedOn"><inheritdoc/></param>
/// <param name="subsetTechnique">Indicates the subset technique used.</param>
public sealed partial class DirectSubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	[PrimaryConstructorParameter] scoped ref readonly CellMap subsetCells,
	[PrimaryConstructorParameter] Mask subsetDigitsMask,
	[PrimaryConstructorParameter] House subsetHouse,
	[PrimaryConstructorParameter] scoped ref readonly CellMap interim,
	[PrimaryConstructorParameter] Mask interimDigitsMask,
	SingleSubtype subtype,
	Technique basedOn,
	[PrimaryConstructorParameter] Technique subsetTechnique
) : ComplexSingleStep(conclusions, views, options, cell, digit, subtype, basedOn, [[subsetTechnique]]), ISizeTrait
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
	protected override int PrefixNameLength
	{
		[DoesNotReturn]
		get => throw new NotImplementedException();
	}

	/// <inheritdoc/>
	protected override string PrefixName
	{
		[DoesNotReturn]
		get => throw new NotImplementedException();
	}

	private string CellsStr => Options.Converter.CellConverter(SubsetCells);

	private string HouseStr => Options.Converter.HouseConverter(1 << SubsetHouse);

	private string InterimCellStr => Options.Converter.CellConverter(Interim);

	private string InterimDigitStr => Options.Converter.DigitConverter(InterimDigitsMask);

	private string TechniqueNameStr => BasedOn.GetName(ResultCurrentCulture);

	private string DigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string SubsetNameStr => SubsetTechnique.GetName(ResultCurrentCulture);
}
