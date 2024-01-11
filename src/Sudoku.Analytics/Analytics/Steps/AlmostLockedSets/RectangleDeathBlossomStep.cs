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
	[RecordParameter] scoped ref readonly CellMap pattern,
	[RecordParameter] bool isAvoidable,
	[RecordParameter] RectangleBlossomBranchCollection branches,
	[RecordParameter] Mask zDigitsMask
) : AlmostLockedSetsStep(conclusions, views, options), IEquatableStep<RectangleDeathBlossomStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.RectangleDeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PatternStr, BranchesStr]), new(ChineseLanguage, [PatternStr, BranchesStr])];

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(ExtraDifficultyFactorNames.Avoidable, .1M),
			new(ExtraDifficultyFactorNames.Petals, A002024(Branches.Count) * .1M)
		];

	private string PatternStr => Options.Converter.CellConverter(Pattern);

	private string BranchesStr
		=> string.Join(
			ResourceDictionary.Get("Comma", ResultCurrentCulture),
			[.. from b in Branches select $"{Options.Converter.CandidateConverter([b.Key])} - {b.AlsPattern}"]
		);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<RectangleDeathBlossomStep>.operator ==(RectangleDeathBlossomStep left, RectangleDeathBlossomStep right)
		=> (left.Pattern, left.IsAvoidable, left.Branches) == (right.Pattern, right.IsAvoidable, right.Branches);
}
