namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Alternating Inference Chain</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Chain">Indicates the whole chain.</param>
/// <param name="XEnabled">Indicates whether the X-chain is enabled.</param>
/// <param name="YEnabled">Indicates whether the Y-chain is enabled.</param>
public sealed record class AlternatingInferenceChainStep(
	ConclusionList Conclusions, ViewList Views, AlternatingInferenceChain Chain, bool XEnabled, bool YEnabled) :
	ChainStep(Conclusions, Views),
	IChainStep,
	IChainLikeStep,
	IStepWithPhasedDifficulty
{
	/// <summary>
	/// Indicates the shared bucket that is used for checking X-Chains.
	/// </summary>
	private static readonly bool[] Bucket = { false, false, false, false, false, false, false, false, false };


	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty
		=> TechniqueCode switch
		{
			Technique.MWing => 4.5M,
			Technique.GroupedMWing => 4.6M,
			Technique.SplitWing or Technique.HybridWing or Technique.LocalWing => 4.8M,
			Technique.GroupedSplitWing or Technique.GroupedHybridWing or Technique.GroupedLocalWing => 4.9M,
			Technique.PurpleCow => 4.9M,
			Technique.GroupedPurpleCow => 5.0M,
			_ => XEnabled && YEnabled ? 5.0M : 4.6M
		};

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			("Length", IsIrregularWing ? 0 : IChainLikeStep.GetExtraDifficultyByLength(FlatComplexity)),
			("Grouped", Chain.IsGrouped ? Chain.RealChainNodes.Sum(NodeDifficultySelector) : 0)
		};

	/// <inheritdoc/>
	public override int FlatComplexity => Chain.RealChainNodes.Length;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup
		=> this switch { { IsIrregularWing: true } => TechniqueGroup.Wing, _ => TechniqueGroup.AlternatingInferenceChain };

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => IsIrregularWing ? TechniqueTags.Wings : TechniqueTags.LongChaining;

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> this switch
		{
			{ Chain: { IsGrouped: true, Count: 3 } } => Technique.GroupedXyWing,
			{ Chain: { IsContinuousNiceLoop: true, IsGrouped: var isGrouped } }
				=> isGrouped ? Technique.GroupedContinuousNiceLoop : Technique.ContinuousNiceLoop,
			{ IsXChain: true, Chain: { IsGrouped: var isGrouped, IsContinuousNiceLoop: var isCnl } }
				=> (isGrouped, isCnl) switch
				{
					(true, true) => Technique.GroupedFishyCycle,
					(true, false) => Technique.GroupedXChain,
					(false, true) => Technique.FishyCycle,
					_ => Technique.XChain
				},
			{ IsWWing: true, Chain.IsGrouped: var isGrouped } => isGrouped ? Technique.GroupedWWing : Technique.WWing,
			{ IsMWing: true, Chain.IsGrouped: var isGrouped } => isGrouped ? Technique.GroupedMWing : Technique.MWing,
			{ IsSplitWing: true, Chain.IsGrouped: var isGrouped }
				=> isGrouped ? Technique.GroupedSplitWing : Technique.SplitWing,
			{ IsHybridWing: true, Chain.IsGrouped: var isGrouped }
				=> isGrouped ? Technique.GroupedHybridWing : Technique.HybridWing,
			{ IsLocalWing: true, Chain.IsGrouped: var isGrouped }
				=> isGrouped ? Technique.GroupedLocalWing : Technique.LocalWing,
			{ Chain: { IsGrouped: var isGrouped, IsIrregularWing: true } }
				=> isGrouped ? Technique.GroupedPurpleCow : Technique.PurpleCow,
			{
				Chain:
				{
					RealChainNodes: [{ Digit: var a }, .., { Digit: var b }],
					IsGrouped: var isGrouped
				},
				IsXyChain: var isXy
			} when a == b => (isXy, isGrouped) switch
			{
				(true, _) => Technique.XyChain,
				(_, true) => Technique.GroupedAlternatingInferenceChain,
				_ => Technique.AlternatingInferenceChain
			},
			{ Chain.IsGrouped: var isGrouped, Conclusions.Length: var conclusionLength } => conclusionLength switch
			{
				1 => isGrouped ? Technique.GroupedDiscontinuousNiceLoop : Technique.DiscontinuousNiceLoop,
				2 => isGrouped ? Technique.GroupedXyXChain : Technique.XyXChain,
				_ => isGrouped ? Technique.GroupedAlternatingInferenceChain : Technique.AlternatingInferenceChain
			}
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => IsIrregularWing ? DifficultyLevel.Hard : DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity
		=> TechniqueCode switch
		{
			Technique.HybridWing or Technique.XChain or Technique.XyChain or Technique.XyXChain => Rarity.Often,
			_ => Rarity.Sometimes
		};

	/// <inheritdoc/>
	public override ChainTypeCode SortKey => Enum.Parse<ChainTypeCode>(TechniqueCode.ToString());

	/// <summary>
	/// Indicates whether the current chain is irregular wing, and not grouped.
	/// </summary>
	private bool IsIrregularWing
		=> TechniqueCode is Technique.WWing or Technique.MWing or Technique.SplitWing or Technique.HybridWing or Technique.LocalWing or Technique.PurpleCow;

	/// <summary>
	/// Indicates whether the specified chain is an XY-Chain.
	/// </summary>
	private bool IsXyChain
	{
		get
		{
			var realChainNodes = Chain.RealChainNodes;
			for (int i = 0, count = realChainNodes.Length; i < count; i += 2)
			{
				if ((realChainNodes[i].Cells, realChainNodes[i + 1].Cells) is not ([var c1], [var c2]) || c1 != c2)
				{
					return false;
				}
			}

			return true;
		}
	}

	/// <summary>
	/// Indicates whether the specified chain is an X-Chain.
	/// </summary>
	private bool IsXChain
	{
		get
		{
			Array.Fill(Bucket, false);
			foreach (var node in Chain.RealChainNodes)
			{
				Bucket[node.Digit] = true;
			}

			return Bucket.Count(static value => value) == 1;
		}
	}

#pragma warning disable IDE0055
	/// <summary>
	/// Indicates whether the chain is W-Wing (<c>(x = y) - y = y - (y = x)</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsWWing
		=> Chain.RealChainNodes switch
		{
			[
				SoleCandidateNode { Candidate: var a },
				SoleCandidateNode { Candidate: var b },
				{ Digit: var digit3 },
				{ Digit: var digit4 },
				SoleCandidateNode { Candidate: var e },
				SoleCandidateNode { Candidate: var f }
			] when a / 9 == b / 9 && e / 9 == f / 9 && a % 9 == f % 9
				&& b % 9 == digit3 && digit3 == digit4 && digit4 == e % 9 => true,
			_ => false
		};

	/// <summary>
	/// Indicates whether the chain is M-Wing (<c>(x = y) - y = (y - x) = x</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsMWing
		=> Chain.RealChainNodes switch
		{
			[
				SoleCandidateNode { Candidate: var a },
				SoleCandidateNode { Candidate: var b },
				{ Digit : var digit3 },
				SoleCandidateNode { Candidate: var d },
				SoleCandidateNode { Candidate: var e },
				{ Digit: var digit6 }
			] when a / 9 == b / 9 && d / 9 == e / 9
				&& b % 9 == digit3 && digit3 == d % 9 && a % 9 == e % 9 && e % 9 == digit6 => true,
			[
				{ Digit: var digit1 },
				SoleCandidateNode { Candidate: var b },
				SoleCandidateNode { Candidate: var c },
				{ Digit: var digit4 },
				SoleCandidateNode { Candidate: var e },
				SoleCandidateNode { Candidate: var f }
			] when b / 9 == c / 9 && e / 9 == f / 9
				&& b % 9 == c % 9 && c % 9 == digit4 && digit1 == e % 9 && e % 9 == f % 9 => true,
			_ => false
		};

	/// <summary>
	/// Indicates whether the chain is Split-Wing (<c>x = x - (x = y) - y = y</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsSplitWing
		=> Chain.RealChainNodes switch
		{
			[
				{ Digit: var digit1 },
				{ Digit: var digit2 },
				SoleCandidateNode { Candidate: var c },
				SoleCandidateNode { Candidate: var d },
				{ Digit: var digit5 },
				{ Digit: var digit6 }
			] when digit1 == digit2 && digit2 == c % 9 && d % 9 == digit5 && digit5 == digit6 && c / 9 == d / 9 => true,
			_ => false
		};

	/// <summary>
	/// Indicates whether the chain is Hybrid-Wing.
	/// This wing has two types:
	/// <list type="bullet">
	/// <item><c>(x = y) - y = (y - z) = z</c></item>
	/// <item><c>(x = y) - (y = z) - z = z</c></item>
	/// </list>
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsHybridWing
		=> Chain.RealChainNodes switch
		{
			[
				SoleCandidateNode { Candidate: var a },
				SoleCandidateNode { Candidate: var b },
				{ Digit: var digit3 },
				SoleCandidateNode { Candidate: var d },
				SoleCandidateNode { Candidate: var e },
				{ Digit: var digit6 }
			] when a / 9 == b / 9 && d / 9 == e / 9 && b % 9 == digit3 && digit3 == d % 9 && e % 9 == digit6 => true,
			[
				{ Digit: var digit1 },
				SoleCandidateNode { Candidate: var b },
				SoleCandidateNode { Candidate: var c },
				{ Digit: var digit4 },
				SoleCandidateNode { Candidate: var e },
				SoleCandidateNode { Candidate: var f }
			] when b / 9 == c / 9 && e / 9 == f / 9 && e % 9 == digit4 && digit4 == c % 9 && digit1 == b % 9 => true,
			[
				SoleCandidateNode { Candidate: var a },
				SoleCandidateNode { Candidate: var b },
				SoleCandidateNode { Candidate: var c },
				SoleCandidateNode { Candidate: var d },
				{ Digit: var digit5 },
				{ Digit: var digit6 }
			] when a / 9 == b / 9 && c / 9 == d / 9 && b % 9 == c % 9 && d % 9 == digit5 && digit5 == digit6 => true,
			[
				{ Digit: var digit1 },
				{ Digit: var digit2 },
				SoleCandidateNode { Candidate: var c },
				SoleCandidateNode { Candidate: var d },
				SoleCandidateNode { Candidate: var e },
				SoleCandidateNode { Candidate: var f }
			] when c / 9 == d / 9 && e / 9 == f / 9 && digit1 == digit2 && digit2 == c % 9 && d % 9 == e % 9 => true,
			_ => false
		};

	/// <summary>
	/// Indicates whether the chain is Local-Wing (<c>x = (x - z) = (z - y) = y</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsLocalWing
		=> Chain.RealChainNodes switch
		{
			[
				{ Digit: var digit1 },
				SoleCandidateNode { Candidate: var b },
				SoleCandidateNode { Candidate: var c },
				SoleCandidateNode { Candidate: var d },
				SoleCandidateNode { Candidate: var e },
				{ Digit: var digit6 }
			] when b / 9 == c / 9 && d / 9 == e / 9 && digit1 == b % 9 && c % 9 == d % 9 && e % 9 == digit6 => true,
			_ => false
		};
#pragma warning restore IDE0055

	[FormatItem]
	internal string ChainStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Chain.ToString();
	}


	/// <summary>
	/// The node difficulty selector.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <returns>The difficulty of the node.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static decimal NodeDifficultySelector(Node node)
		=> node switch
		{
			SoleCandidateNode => 0M,
			LockedCandidatesNode => .1M,
			AlmostLockedSetNode => .2M
		};
}
