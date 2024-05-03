namespace Sudoku.Analytics.Steps;

public partial class HiddenSubsetStep
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 4;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedHiddenPair,
			(_, 2) => Technique.HiddenPair,
			(true, 3) => Technique.LockedHiddenTriple,
			(_, 3) => Technique.HiddenTriple,
			(_, 4) => Technique.HiddenQuadruple
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, HouseStr]), new(ChineseLanguage, [DigitStr, HouseStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new HiddenSubsetSizeFactor(), new HiddenSubsetIsLockedFactor()];

	private string DigitStr => Options.Converter.DigitConverter(DigitsMask);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
