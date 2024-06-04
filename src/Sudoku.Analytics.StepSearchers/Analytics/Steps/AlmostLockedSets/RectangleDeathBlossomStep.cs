namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom (Rectangle Blooming)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern">Indicates the pattern used.</param>
/// <param name="isAvoidable">Indicates whether the pattern is an avoidable rectangle.</param>
/// <param name="branches">Indicates the branches.</param>
/// <param name="zDigitsMask">Indicates the digits mask as eliminations.</param>
public sealed partial class RectangleDeathBlossomStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] ref readonly CellMap pattern,
	[PrimaryConstructorParameter] bool isAvoidable,
	[PrimaryConstructorParameter] RectangleBlossomBranchCollection branches,
	[PrimaryConstructorParameter] Mask zDigitsMask
) :
	DeathBlossomBaseStep(conclusions, views, options),
	IBranchTrait,
	IDeathBlossomCollection<RectangleBlossomBranchCollection, Candidate>
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 3;

	/// <inheritdoc/>
	public override Technique Code => Technique.RectangleDeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PatternStr, BranchesStr]), new(ChineseLanguage, [PatternStr, BranchesStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [new RectangleDeathBlossomPetalsCountFactor(), new RectangleDeathBlossomIsAvoidableFactor()];

	/// <inheritdoc/>
	int IBranchTrait.BranchesCount => Branches.Count;

	private string PatternStr => Options.Converter.CellConverter(Pattern);

	private string BranchesStr
		=> string.Join(
			ResourceDictionary.Get("Comma", ResultCurrentCulture),
#if !NET9_0_OR_GREATER
			[
			..
#endif
			from b in Branches select $"{Options.Converter.CandidateConverter(b.Key)} - {b.AlsPattern}"
#if !NET9_0_OR_GREATER
			]
#endif
		);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is RectangleDeathBlossomStep comparer
		&& (Pattern, IsAvoidable, Branches) == (comparer.Pattern, comparer.IsAvoidable, comparer.Branches);
}
