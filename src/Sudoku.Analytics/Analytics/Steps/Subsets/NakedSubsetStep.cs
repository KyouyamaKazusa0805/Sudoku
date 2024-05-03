namespace Sudoku.Analytics.Steps;

public partial class NakedSubsetStep
{
	/// <inheritdoc/>
	public override Technique Code
		=> (IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedPair,
			(false, 2) => Technique.NakedPairPlus,
			(_, 2) => Technique.NakedPair,
			(true, 3) => Technique.LockedTriple,
			(false, 3) => Technique.NakedTriplePlus,
			(_, 3) => Technique.NakedTriple,
			(false, 4) => Technique.NakedQuadruplePlus,
			(null, 4) => Technique.NakedQuadruple
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, HouseStr]), new(ChineseLanguage, [DigitsStr, HouseStr, SubsetName])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new NakedSubsetSizeFactor(), new NakedSubsetIsLockedFactor()];

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);

	private string SubsetName => ResourceDictionary.Get($"SubsetNamesSize{Size}", ResultCurrentCulture);
}
