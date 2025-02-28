namespace Sudoku.Analytics.Steps.AlmostLockedSets;

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
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] in CellMap pattern,
	[Property] bool isAvoidable,
	[Property] RectangleBlossomBranchCollection branches,
	[Property] Mask zDigitsMask
) :
	DeathBlossomStep(conclusions, views, options),
	IBranchTrait,
	IDeathBlossomCollection<RectangleBlossomBranchCollection, Candidate>
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 3;

	/// <inheritdoc/>
	public override Technique Code => Technique.RectangleDeathBlossom;

	/// <inheritdoc/>
	public override Mask DigitsUsed => Branches.DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [PatternStr, BranchesStr(SR.EnglishLanguage)]),
			new(SR.ChineseLanguage, [PatternStr, BranchesStr(SR.ChineseLanguage)])
		];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_RectangleDeathBlossomPetalsCountFactor",
				[nameof(IBranchTrait.BranchesCount)],
				GetType(),
				static args => OeisSequences.A002024((int)args![0]!)
			),
			Factor.Create(
				"Factor_RectangleDeathBlossomIsAvoidableFactor",
				[nameof(IsAvoidable)],
				GetType(),
				static args => (bool)args![0]! ? 1 : 0
			)
		];

	/// <inheritdoc/>
	int IBranchTrait.BranchesCount => Branches.Count;

	private string PatternStr => Options.Converter.CellConverter(Pattern);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is RectangleDeathBlossomStep comparer
		&& (Pattern, IsAvoidable, Branches) == (comparer.Pattern, comparer.IsAvoidable, comparer.Branches);

	private string BranchesStr(string cultureName)
	{
		var culture = new CultureInfo(cultureName);
		return string.Join(
			SR.Get("Comma", culture),
			from b in Branches select $"{Options.Converter.CandidateConverter([b.Key])} - {b.Value}"
		);
	}
}
