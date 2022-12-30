namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Alternating Inference Chain</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Chain">Indicates the whole chain.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record AlternatingInferenceChainStep(
	ConclusionList Conclusions,
	ViewList Views,
	AlternatingInferenceChain Chain
) : ChainStep(Conclusions, Views), IChainStep, IChainLikeStep, IStepWithPhasedDifficulty
{
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
			Technique.XChain => 4.6M,
			_ => 5.0M
		};

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			(
				PhasedDifficultyRatingKinds.Length,
				IsIrregularWing ? 0 : IChainLikeStep.GetExtraDifficultyByLength(FlatComplexity)
			),
#if false
			(
				PhasedDifficultyRatingKinds.GroupedChains,
				Chain.IsGrouped ? Chain.RealChainNodes.Sum(NodeDifficultySelector) : 0
			)
#endif
		};

	/// <inheritdoc/>
	public override int FlatComplexity => Chain.RealChainNodes.Length;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup
		=> this switch
		{
			{ IsIrregularWing: true } => TechniqueGroup.Wing,
			_ => TechniqueGroup.AlternatingInferenceChain
		};

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags
		=> IsIrregularWing ? TechniqueTags.Wings : TechniqueTags.LongChaining;

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> this switch
		{
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
			{ Chain: { IsGrouped: true, Count: 3 } } => Technique.GroupedXyWing,
			{ IsWWing: true, Chain.IsGrouped: var isGrouped } => isGrouped ? Technique.GroupedWWing : Technique.WWing,
			{ IsMWing: true, Chain.IsGrouped: var isGrouped } => isGrouped ? Technique.GroupedMWing : Technique.MWing,
			{ IsSplitWing: true, Chain.IsGrouped: var isGrouped } => isGrouped ? Technique.GroupedSplitWing : Technique.SplitWing,
			{ IsHybridWing: true, Chain.IsGrouped: var isGrouped } => isGrouped ? Technique.GroupedHybridWing : Technique.HybridWing,
			{ IsLocalWing: true, Chain.IsGrouped: var isGrouped } => isGrouped ? Technique.GroupedLocalWing : Technique.LocalWing,
			{ Chain: { IsGrouped: var isGrouped, IsIrregularWing: true } } => isGrouped ? Technique.GroupedPurpleCow : Technique.PurpleCow,
			{ Chain.IsGrouped: false, IsRemotePair: true } => Technique.RemotePair,
			{ Chain.IsAlmostLockedSetsOnly: true } => Technique.AlmostLockedSetsChain,
			{ Chain.IsAlmostHiddenSetsOnly: true } => Technique.AlmostHiddenSetsChain,
			{ Chain.HasNodeCollision: true } => Technique.NodeCollision,
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
			{ Chain.IsGrouped: var isGrouped, Conclusions.Length: var conclusionLength }
				=> conclusionLength switch
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
			for (var i = 0; i < realChainNodes.Length; i += 2)
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
	private unsafe bool IsXChain
	{
		get
		{
			var bucket = stackalloc[] { false, false, false, false, false, false, false, false, false };
			foreach (var node in Chain.RealChainNodes)
			{
				bucket[node.Digit] = true;
			}

			var count = 0;
			for (var i = 0; i < 9; i++)
			{
				if (bucket[i] && ++count > 1)
				{
					return false;
				}
			}

			return count == 1;
		}
	}

	/// <summary>
	/// Indicates whether the specified chain is a remote pair.
	/// </summary>
	private unsafe bool IsRemotePair
	{
		get
		{
			if (!IsXyChain)
			{
				return false;
			}

			var bucket = stackalloc[] { false, false, false, false, false, false, false, false, false };
			foreach (var node in Chain.RealChainNodes)
			{
				bucket[node.Digit] = true;
			}

			var count = 0;
			for (var i = 0; i < 9; i++)
			{
				if (bucket[i] && ++count > 2)
				{
					return false;
				}
			}

			return count == 2;
		}
	}

#pragma warning disable format
	/// <summary>
	/// Indicates whether the chain is W-Wing (<c>(x = y) - y = y - (y = x)</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsWWing
		=> Chain.RealChainNodes switch
		{
			[
				{ Cells: [var c1], Digit: var d1 },
				{ Cells: [var c2], Digit: var d2 },
				{ Digit: var d3 },
				{ Digit: var d4 },
				{ Cells: [var c5], Digit: var d5 },
				{ Cells: [var c6], Digit: var d6 }
			] when c1 == c2 && c5 == c6 && d1 == d6 && d2 == d3 && d3 == d4 && d4 == d5 => true,
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
				{ Cells: [var c1], Digit: var d1 },
				{ Cells: [var c2], Digit: var d2 },
				{ Digit: var d3 },
				{ Cells: [var c4], Digit: var d4 },
				{ Cells: [var c5], Digit: var d5 },
				{ Digit: var d6 }
			] when c1 == c2 && c4 == c5 && d2 == d3 && d3 == d4 && d1 == d5 && d5 == d6 => true,
			[
				{ Digit: var d1 },
				{ Cells: [var c2], Digit: var d2 },
				{ Cells: [var c3], Digit: var d3 },
				{ Digit: var d4 },
				{ Cells: [var c5], Digit: var d5 },
				{ Cells: [var c6], Digit: var d6 }
			] when c2 == c3 && c5 == c6 && d2 == d3 && d3 == d4 && d1 == d5 && d5 == d6 => true,
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
				{ Digit: var d1 },
				{ Digit: var d2 },
				{ Cells: [var c3], Digit: var d3 },
				{ Cells: [var c4], Digit: var d4 },
				{ Digit: var d5 },
				{ Digit: var d6 }
			] when d1 == d2 && d2 == d3 && d4 == d5 && d5 == d6 && c3 == c4 => true,
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
				{ Cells: [var c1] },
				{ Cells: [var c2], Digit: var d2 },
				{ Digit: var d3 },
				{ Cells: [var c4], Digit: var d4 },
				{ Cells: [var c5], Digit: var d5 },
				{ Digit: var d6 }
			] when c1 == c2 && c4 == c5 && d2 == d3 && d3 == d4 && d5 == d6 => true,
			[
				{ Digit: var d1 },
				{ Cells: [var c2], Digit: var d2 },
				{ Cells: [var c3], Digit: var d3 },
				{ Digit: var d4 },
				{ Cells: [var c5], Digit: var d5 },
				{ Cells: [var c6] }
			] when c2 == c3 && c5 == c6 && d5 == d4 && d4 == d3 && d1 == d2 => true,
			[
				{ Cells: [var c1] },
				{ Cells: [var c2], Digit: var d2 },
				{ Cells: [var c3], Digit: var d3 },
				{ Cells: [var c4], Digit: var d4 },
				{ Digit: var d5 },
				{ Digit: var d6 }
			] when c1 == c2 && c3 == c4 && d2 == d3 && d4 == d5 && d5 == d6 => true,
			[
				{ Digit: var d1 },
				{ Digit: var d2 },
				{ Cells: [var c3], Digit: var d3 },
				{ Cells: [var c4], Digit: var d4 },
				{ Cells: [var c5], Digit: var d5 },
				{ Cells: [var c6] }
			] when c3 == c4 && c5 == c6 && d1 == d2 && d2 == d3 && d4 == d5 => true,
			_ => false
		};

	/// <summary>
	/// Indicates whether the chain is Local-Wing (<c>x = (x - z) = (z - y) = y</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsLocalWing => Chain.RealChainNodes switch
	{
		[
			{ Digit: var d1 },
			{ Cells: [var c2], Digit: var d2 },
			{ Cells: [var c3], Digit: var d3 },
			{ Cells: [var c4], Digit: var d4 },
			{ Cells: [var c5], Digit: var d5 },
			{ Digit: var d6 }
		] when c2 == c3 && c4 == c5 && d1 == d2 && d3 == d4 && d5 == d6 => true,
		_ => false
	};
#pragma warning restore format

	[ResourceTextFormatter]
	internal string ChainStr() => Chain.ToString();


#if false
	/// <summary>
	/// The node difficulty selector.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <returns>The difficulty of the node.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static decimal NodeDifficultySelector(Node node)
		=> node.Type switch
		{
			NodeType.Sole => 0,
			NodeType.LockedCandidates => .1M,
			NodeType.AlmostLockedSets => .2M,
			NodeType.AlmostHiddenSets => .3M,
			NodeType.AlmostUniqueRectangle => .4M
		};
#endif
}
