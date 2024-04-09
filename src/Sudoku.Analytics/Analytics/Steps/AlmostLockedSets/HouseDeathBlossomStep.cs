namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom (House Blooming)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="house">Indicates the pivot house.</param>
/// <param name="digit">Indicates the digit.</param>
/// <param name="branches">Indicates the branches.</param>
/// <param name="zDigitsMask">Indicates the digits mask as eliminations.</param>
public sealed partial class HouseDeathBlossomStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] House house,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] HouseBlossomBranchCollection branches,
	[PrimaryConstructorParameter] Mask zDigitsMask
) : DeathBlossomBaseStep(conclusions, views, options), IDeathBlossomCollection<HouseBlossomBranchCollection, House>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override Technique Code => Technique.HouseDeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [HouseStr, BranchesStr]), new(ChineseLanguage, [HouseStr, BranchesStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new HouseDeathBlossomPetalsCountFactor(Options)];

	private string HouseStr => Options.Converter.HouseConverter(1 << House);

	private string BranchesStr
		=> string.Join(
			ResourceDictionary.Get("Comma", ResultCurrentCulture),
			[.. from b in Branches select $"{Options.Converter.CellConverter([b.Key])} - {b.AlsPattern}"]
		);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is HouseDeathBlossomStep comparer
		&& (House, Digit, Branches) == (comparer.House, comparer.Digit, comparer.Branches);
}
