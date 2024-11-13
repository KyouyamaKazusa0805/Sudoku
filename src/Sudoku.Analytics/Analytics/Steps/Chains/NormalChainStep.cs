namespace Sudoku.Analytics.Steps.Chains;

/// <summary>
/// Provides with a step that is a <b>(Grouped) Chain</b> or <b>(Grouped) Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
public class NormalChainStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	NamedChain pattern
) : PatternBasedChainStep(conclusions, views, options, pattern)
{
	/// <summary>
	/// Indicates the sort key that can be used as chaining comparison.
	/// </summary>
	public int SortKey
		=> (IsLoop ? 2000 : 0) + (IsCannibalistic ? 1000 : 0) + (IsGrouped ? 500 : 0) + Casted.GetSortKey(Conclusions.AsSet());

	/// <inheritdoc/>
	public sealed override bool IsMultiple => false;

	/// <inheritdoc/>
	public sealed override bool IsDynamic => false;

	/// <summary>
	/// Indicates whether the chain is cannibalistic,
	/// meaning at least one of a candidate inside a node used in the pattern is also a conclusion.
	/// </summary>
	public bool IsCannibalistic => Casted.OverlapsWithConclusions(Conclusions.AsSet());

	/// <summary>
	/// Indicates whether the chain uses at least one grouped node.
	/// </summary>
	public bool IsGrouped => Pattern.IsGrouped;

	/// <summary>
	/// Indicates whether the chain pattern forms a Continuous Nice Loop or Grouped Continuous Nice Loop.
	/// </summary>
	public bool IsLoop => Pattern is ContinuousNiceLoop;

	/// <inheritdoc/>
	public override int BaseDifficulty => 60;

	/// <inheritdoc/>
	public sealed override int Complexity => Casted.Length;

	/// <inheritdoc/>
	public override Technique Code => Casted.GetTechnique(Conclusions.AsSet());

	/// <inheritdoc/>
	public override Mask DigitsUsed => Casted.DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [ChainString]), new(SR.ChineseLanguage, [ChainString])];

	/// <inheritdoc/>
	public sealed override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_ChainLengthFactor",
				[nameof(Complexity)],
				GetType(),
				static args => ChainingLength.GetLengthDifficulty((int)args![0]!)
			),
			Factor.Create(
				"Factor_ChainGroupedFactor",
				[nameof(IsGrouped)],
				GetType(),
				static args => (bool)args![0]! ? 2 : 0
			),
			Factor.Create(
				"Factor_ChainGroupedNodeFactor",
				[nameof(Pattern)],
				GetType(),
				static args =>
				{
					var result = 0;
					var p = (Chain)args![0]!;
					foreach (var link in p.Links)
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
					return result;
				}
			)
		];

	private protected string ChainString => Casted.ToString(new ChainFormatInfo(Options.Converter));

	private protected NamedChain Casted => (NamedChain)Pattern;


	/// <inheritdoc/>
	public sealed override bool Equals([NotNullWhen(true)] Step? other)
		=> other is NormalChainStep comparer && Casted.Equals(comparer.Casted);

	/// <inheritdoc/>
	public sealed override int CompareTo(Step? other)
		=> other is NormalChainStep comparer
			? SortKey.CompareTo(comparer.SortKey) is var result and not 0
				? result
				: Casted.CompareTo(comparer.Casted)
			: -1;
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Indicates the sort key dictionary.
	/// </summary>
	private static readonly Dictionary<Technique, int> SortKeyDictionary = new()
	{
		// 3-chain
		{ Technique.Skyscraper, 1 },
		{ Technique.TwoStringKite, 2 },
		{ Technique.TurbotFish, 3 },
		{ Technique.GroupedSkyscraper, 4 },
		{ Technique.GroupedTwoStringKite, 5 },
		{ Technique.GroupedTurbotFish, 6 },

		// 5-chain
		{ Technique.WWing, 101 },
		{ Technique.MWing, 102 },
		{ Technique.SWing, 103 },
		{ Technique.LWing, 104 },
		{ Technique.HWing, 105 },
		{ Technique.PurpleCow, 106 },
		{ Technique.GroupedWWing, 107 },
		{ Technique.GroupedMWing, 108 },
		{ Technique.GroupedSWing, 109 },
		{ Technique.GroupedLWing, 110 },
		{ Technique.GroupedHWing, 111 },
		{ Technique.GroupedPurpleCow, 112 },

		// Others
		{ Technique.FishyCycle, 201 },
		{ Technique.XyCycle, 202 },
		{ Technique.ContinuousNiceLoop, 203 },
		{ Technique.XChain, 204 },
		{ Technique.XyChain, 205 },
		{ Technique.XyXChain, 206 },
		{ Technique.SelfConstraint, 207 },
		{ Technique.AlternatingInferenceChain, 208 },
		{ Technique.DiscontinuousNiceLoop, 209 },
		{ Technique.GroupedFishyCycle, 211 },
		{ Technique.GroupedXyCycle, 212 },
		{ Technique.GroupedContinuousNiceLoop, 213 },
		{ Technique.GroupedXChain, 214 },
		{ Technique.GroupedXyChain, 215 },
		{ Technique.GroupedXyXChain, 216 },
		{ Technique.GroupedSelfConstraint, 217 },
		{ Technique.NodeCollision, 218 },
		{ Technique.GroupedAlternatingInferenceChain, 219 },
		{ Technique.GroupedDiscontinuousNiceLoop, 220 },
		{ Technique.SinglyLinkedAlmostLockedSetsXzRule, 221 },
		{ Technique.DoublyLinkedAlmostLockedSetsXzRule, 222 },
		{ Technique.AlmostLockedSetsXyWing, 223 },
		{ Technique.AlmostLockedSetsWWing, 224 },
		{ Technique.GroupedAlmostLockedSetsWWing, 225 },
		{ Technique.AlmostLockedSetsChain, 226 }
	};


	/// <summary>
	/// Try to get sort key of the pattern.
	/// </summary>
	/// <param name="this">The pattern.</param>
	/// <param name="conclusions">Indicates conclusions to be used.</param>
	/// <returns>The pattern sort key.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetSortKey(this NamedChain @this, ConclusionSet conclusions)
		=> SortKeyDictionary[@this.GetTechnique(conclusions)];
}
