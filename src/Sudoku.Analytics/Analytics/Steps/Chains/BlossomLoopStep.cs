namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Blossom Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern">Indicates the backing pattern.</param>
public sealed partial class BlossomLoopStep(
	ReadOnlyMemory<Conclusion> conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] BlossomLoop pattern
) : ChainStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsMultiple => true;

	/// <inheritdoc/>
	public override bool IsDynamic => false;

	/// <summary>
	/// Indicates whether the pattern uses grouped nodes.
	/// </summary>
	public bool IsGrouped => Pattern.Exists(static chain => chain.IsGrouped);

	/// <inheritdoc/>
	public override int Complexity => Pattern.Complexity;

	/// <inheritdoc/>
	public override int BaseDifficulty => 65;

	/// <inheritdoc/>
	public override Technique Code => Technique.BlossomLoop;

	/// <inheritdoc/>
	public override Mask DigitsUsed => Pattern.DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [BurredLoopStr]), new(SR.ChineseLanguage, [BurredLoopStr])];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_BlossomLoopGroupedFactor",
				[nameof(IsGrouped)],
				GetType(),
				static args => (bool)args![0]! ? 2 : 0
			),
			Factor.Create(
				"Factor_BlossomLoopGroupedNodeFactor",
				[nameof(Pattern)],
				GetType(),
				static args =>
				{
					var result = 0;
					foreach (var branch in ((BlossomLoop)args![0]!).Values)
					{
						foreach (var link in branch.Links)
						{
							result += link.GroupedLinkPattern switch
							{
								AlmostLockedSetPattern => 2,
								AlmostHiddenSetPattern => 3,
								UniqueRectanglePattern => 4,
								FishPattern => 6,
								XyzWingPattern => 8,
								null when link.FirstNode.IsGroupedNode || link.SecondNode.IsGroupedNode => 1,
								_ => 0
							};
						}
					}
					return result;
				}
			),
			Factor.Create(
				"Factor_BlossomLoopLengthFactor",
				[nameof(Complexity)],
				GetType(),
				static args => ChainingLength.GetLengthDifficulty((int)args![0]!)
			)
		];

	private string BurredLoopStr => Pattern.ToString(new ChainFormatInfo(Options.Converter));


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is BlossomLoopStep comparer && Pattern.Equals(comparer.Pattern);

	/// <inheritdoc/>
	public override int CompareTo(Step? other)
		=> other is BlossomLoopStep comparer ? Pattern.CompareTo(comparer.Pattern) : -1;
}
