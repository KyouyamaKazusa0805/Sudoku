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
) : DeathBlossomBaseStep(conclusions, views, options), IBranchTrait, IDeathBlossomCollection<HouseBlossomBranchCollection, House>
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override Technique Code => Technique.HouseDeathBlossom;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [HouseStr, BranchesStr(SR.EnglishLanguage)]),
			new(SR.ChineseLanguage, [HouseStr, BranchesStr(SR.ChineseLanguage)])
		];

	/// <inheritdoc/>
	public override FactorArray Factors => [new HouseDeathBlossomPetalsCountFactor()];

	/// <inheritdoc/>
	int IBranchTrait.BranchesCount => Branches.Count;

	private string HouseStr => Options.Converter.HouseConverter(1 << House);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is HouseDeathBlossomStep comparer
		&& (House, Digit, Branches) == (comparer.House, comparer.Digit, comparer.Branches);

	private string BranchesStr(string cultureName)
	{
		var culture = new CultureInfo(cultureName);
		return string.Join(
			SR.Get("Comma", culture),
			from b in Branches select $"{Options.Converter.CellConverter(in b.Key.AsCellMap())} - {b.Value}"
		);
	}
}
