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
	[PrimaryConstructorParameter] scoped ref readonly CellMap pattern,
	[PrimaryConstructorParameter] bool isAvoidable,
	[PrimaryConstructorParameter] RectangleBlossomBranchCollection branches,
	[PrimaryConstructorParameter] Mask zDigitsMask
) : DeathBlossomBaseStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .3M;

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
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is RectangleDeathBlossomStep comparer
		&& (Pattern, IsAvoidable, Branches) == (comparer.Pattern, comparer.IsAvoidable, comparer.Branches);
}
