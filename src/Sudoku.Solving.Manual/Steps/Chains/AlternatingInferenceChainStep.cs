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
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty
		=> TechniqueCode switch
		{
			Technique.MWing => 4.5M,
			Technique.SplitWing or Technique.HybridWing or Technique.LocalWing => 4.8M,
			_ => XEnabled && YEnabled ? 5.0M : 4.6M
		};

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { ("Length", IsIrregularWing ? 0 : IChainLikeStep.GetExtraDifficultyByLength(FlatComplexity)) };

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
			{ Chain: { IsContinuousNiceLoop: true, IsGrouped: var isGrouped } }
				=> isGrouped ? Technique.GroupedContinuousNiceLoop : Technique.ContinuousNiceLoop,
			{ IsXChain: true, Chain.IsGrouped: var isGrouped }
				=> isGrouped ? Technique.GroupedXChain : Technique.XChain,
			{ IsMWing: true, Chain.IsGrouped: var isGrouped }
				=> isGrouped ? Technique.GroupedMWing : Technique.MWing,
			{ IsSplitWing: true, Chain.IsGrouped: var isGrouped }
				=> isGrouped ? Technique.GroupedSplitWing : Technique.SplitWing,
			{ IsHybridWing: true, Chain.IsGrouped: var isGrouped }
				=> isGrouped ? Technique.GroupedHybridWing : Technique.HybridWing,
			{ IsLocalWing: true, Chain.IsGrouped: var isGrouped }
				=> isGrouped ? Technique.GroupedLocalWing : Technique.LocalWing,
			{ Chain: { Count: 5, IsGrouped: var isGrouped } }
				=> isGrouped ? Technique.GroupedPurpleCow : Technique.PurpleCow,
			{
				Chain: { RealChainNodes: [{ Digit: var a }, .., { Digit: var b }], IsGrouped: var isGrouped },
				IsXyChain: var isXy
			} when a == b
				=> (isXy, isGrouped) switch
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
	/// Indicates whether the current chain is irregular wing.
	/// </summary>
	private bool IsIrregularWing
		=> TechniqueCode is Technique.MWing or Technique.SplitWing or Technique.HybridWing or Technique.LocalWing;

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
	private bool IsXChain => XEnabled && !YEnabled;

	/// <summary>
	/// Indicates whether the chain is M-Wing (<c>(x = y) - y = (y - x) = x</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsMWing
		=> Chain.RealChainNodes is
		[
			SoleCandidateNode { Candidate: var a },
			SoleCandidateNode { Candidate: var b },
			SoleCandidateNode { Candidate: var c },
			SoleCandidateNode { Candidate: var d },
			SoleCandidateNode { Candidate: var e },
			SoleCandidateNode { Candidate: var f }
		]
		&& MWing(a, b, c, d, e, f);

	/// <summary>
	/// Indicates whether the chain is Split-Wing (<c>x = x - (x = y) - y = y</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsSplitWing
		=> Chain.RealChainNodes is
		[
			SoleCandidateNode { Candidate: var a },
			SoleCandidateNode { Candidate: var b },
			SoleCandidateNode { Candidate: var c },
			SoleCandidateNode { Candidate: var d },
			SoleCandidateNode { Candidate: var e },
			SoleCandidateNode { Candidate: var f }
		]
		&& SplitWing(a, b, c, d, e, f);

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
		=> Chain.RealChainNodes is
		[
			SoleCandidateNode { Candidate: var a },
			SoleCandidateNode { Candidate: var b },
			SoleCandidateNode { Candidate: var c },
			SoleCandidateNode { Candidate: var d },
			SoleCandidateNode { Candidate: var e },
			SoleCandidateNode { Candidate: var f }
		]
		&& HybridWing(a, b, c, d, e, f);

	/// <summary>
	/// Indicates whether the chain is Local-Wing (<c>x = (x - z) = (z - y) = y</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsLocalWing
		=> Chain.RealChainNodes is
		[
			SoleCandidateNode { Candidate: var a },
			SoleCandidateNode { Candidate: var b },
			SoleCandidateNode { Candidate: var c },
			SoleCandidateNode { Candidate: var d },
			SoleCandidateNode { Candidate: var e },
			SoleCandidateNode { Candidate: var f }
		]
		&& LocalWing(a, b, c, d, e, f);

	[FormatItem]
	internal string ChainStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Chain.ToString();
	}


	/// <summary>
	/// Determines whether the chain is M-Wing (<c>(x = y) - y = (y - x) = x</c>).
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool MWing(int a, int b, int c, int d, int e, int f)
		=> a / 9 == b / 9 && d / 9 == e / 9 && b % 9 == c % 9 && c % 9 == d % 9 && a % 9 == e % 9 && e % 9 == f % 9
		|| f / 9 == e / 9 && c / 9 == b / 9 && d % 9 == e % 9 && c % 9 == d % 9 && b % 9 == f % 9 && a % 9 == b % 9;

	/// <summary>
	/// Determines whether the chain is Split-Wing (<c>x = x - (x = y) - y = y</c>).
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool SplitWing(int a, int b, int c, int d, int e, int f)
		=> a % 9 == b % 9 && b % 9 == c % 9 && d % 9 == e % 9 && e % 9 == f % 9 && c / 9 == d / 9;

	/// <summary>
	/// Determines whether the chain is Hybrid-Wing.
	/// This wing has two types:
	/// <list type="bullet">
	/// <item><c>(x = y) - y = (y - z) = z</c></item>
	/// <item><c>(x = y) - (y = z) - z = z</c></item>
	/// </list>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HybridWing(int a, int b, int c, int d, int e, int f)
		=> a / 9 == b / 9 && d / 9 == e / 9 && b % 9 == c % 9 && c % 9 == d % 9 && e % 9 == f % 9
		|| e / 9 == f / 9 && b / 9 == c / 9 && d % 9 == e % 9 && c % 9 == d % 9 && a % 9 == b % 9
		|| a / 9 == b / 9 && c / 9 == d / 9 && b % 9 == c % 9 && d % 9 == e % 9 && e % 9 == f % 9
		|| e / 9 == f / 9 && c / 9 == d / 9 && d % 9 == e % 9 && b % 9 == c % 9 && a % 9 == b % 9;

	/// <summary>
	/// Determines whether the chain is Local-Wing (<c>x = (x - z) = (z - y) = y</c>).
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool LocalWing(int a, int b, int c, int d, int e, int f)
		=> b / 9 == c / 9 && d / 9 == e / 9 && a % 9 == b % 9 && c % 9 == d % 9 && e % 9 == f % 9;
}
