namespace Sudoku.Solving.Manual.Steps.Chains;

/// <summary>
/// Provides with a step that is an <b>Alternating Inference Chain</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="XEnabled"><inheritdoc/></param>
/// <param name="YEnabled"><inheritdoc/></param>
/// <param name="Target">Indicates the target cell.</param>
public sealed record AlternatingInferenceChainStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	bool XEnabled,
	bool YEnabled,
	Node Target
) : ChainStep(Conclusions, Views, XEnabled, YEnabled, false, false, false, 0)
{
	/// <inheritdoc/>
	public override decimal Difficulty => TechniqueCode switch
	{
		Technique.MWing => 4.5M,
		Technique.SplitWing or Technique.HybridWing or Technique.LocalWing => 4.8M,
		_ => (XEnabled && YEnabled ? 5.0M : 4.6M) + IChainLikeStep.GetExtraDifficultyByLength(FlatComplexity - 2)
	};

	/// <inheritdoc/>
	public override int FlatComplexity => Target.AncestorsCount;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => this switch
	{
		{ IsMWing: true } => TechniqueGroup.Wing,
		{ IsLocalWing: true } or { IsSplitWing: true } or { IsHybridWing: true } => TechniqueGroup.Wing,
		_ => TechniqueGroup.AlternatingInferenceChain
	};

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => this switch
	{
		{ IsMWing: true } => TechniqueTags.Wings,
		{ IsLocalWing: true } or { IsSplitWing: true } or { IsHybridWing: true } => TechniqueTags.Wings,
		_ => TechniqueTags.LongChaining
	};

	/// <inheritdoc/>
	public override Technique TechniqueCode => this switch
	{
		{ IsXChain: true } => Technique.XChain,
		{ IsMWing: true } => Technique.MWing,
		{ IsSplitWing: true } => Technique.SplitWing,
		{ IsHybridWing: true } => Technique.HybridWing,
		{ IsLocalWing: true } => Technique.LocalWing,
		{ Target.WholeChain: [_, { Digit: var a }, .., { Digit: var b }, _], IsXyChain: var isXy } when a == b =>
			isXy ? Technique.XyChain : Technique.AlternatingInferenceChain,
		_ => Conclusions.Length switch
		{
			1 => Technique.DiscontinuousNiceLoop,
			2 => Technique.XyXChain,
			_ => Technique.AlternatingInferenceChain
		}
	};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel =>
		TechniqueCode is Technique.MWing or Technique.SplitWing or Technique.HybridWing or Technique.LocalWing
			? DifficultyLevel.Hard
			: DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => TechniqueCode switch
	{
		Technique.MWing => Rarity.Sometimes,
		Technique.SplitWing => Rarity.Sometimes,
		Technique.HybridWing => Rarity.Often,
		Technique.LocalWing => Rarity.Sometimes,
		Technique.XChain => Rarity.Often,
		Technique.XyChain => Rarity.Often,
		Technique.XyXChain => Rarity.Often,
		Technique.DiscontinuousNiceLoop => Rarity.Often,
		Technique.AlternatingInferenceChain => Rarity.Often
	};

	/// <inheritdoc/>
	public override ChainTypeCode SortKey => Enum.Parse<ChainTypeCode>(TechniqueCode.ToString());

	/// <summary>
	/// Indicates whether the specified chain is an XY-Chain.
	/// </summary>
	private bool IsXyChain
	{
		get
		{
			if (Views[0].Links is { } links)
			{
				for (int i = 0, count = links.Count; i < count; i += 2)
				{
					var (link, _) = links[i];
					if (link.StartCandidate / 9 != link.EndCandidate / 9)
					{
						goto ReturnFalse;
					}
				}

				return true;
			}

		ReturnFalse:
			return false;
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
	{
		get
		{
			if (FlatComplexity != 8)
			{
				return false;
			}

			var chain = Target.WholeChain;
			var (a, _) = chain[1];
			var (b, _) = chain[2];
			var (c, _) = chain[3];
			var (d, _) = chain[4];
			var (e, _) = chain[5];
			var (f, _) = chain[6];

			return a / 9 == b / 9 && d / 9 == e / 9
				&& b % 9 == c % 9 && c % 9 == d % 9
				&& a % 9 == e % 9 && e % 9 == f % 9
				|| f / 9 == e / 9 && c / 9 == b / 9 // Reverse case.
				&& d % 9 == e % 9 && c % 9 == d % 9
				&& b % 9 == f % 9 && a % 9 == b % 9;
		}
	}

	/// <summary>
	/// Indicates whether the chain is Split-Wing (<c>x = x - (x = y) - y = y</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsSplitWing
	{
		get
		{
			if (FlatComplexity != 8)
			{
				return false;
			}

			var chain = Target.WholeChain;
			var (a, _) = chain[1];
			var (b, _) = chain[2];
			var (c, _) = chain[3];
			var (d, _) = chain[4];
			var (e, _) = chain[5];
			var (f, _) = chain[6];

			return a % 9 == b % 9 && b % 9 == c % 9 // First three nodes hold a same digit.
				&& d % 9 == e % 9 && e % 9 == f % 9 // Last three nodes hold a same digit.
				&& c / 9 == d / 9; // In same cell.
		}
	}

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
	{
		get
		{
			if (FlatComplexity != 8)
			{
				return false;
			}

			var chain = Target.WholeChain;
			var (a, _) = chain[1];
			var (b, _) = chain[2];
			var (c, _) = chain[3];
			var (d, _) = chain[4];
			var (e, _) = chain[5];
			var (f, _) = chain[6];

			return a / 9 == b / 9 && d / 9 == e / 9
				&& b % 9 == c % 9 && c % 9 == d % 9
				&& e % 9 == f % 9
				|| e / 9 == f / 9 && b / 9 == c / 9
				&& d % 9 == e % 9 && c % 9 == d % 9
				&& a % 9 == b % 9
				|| a / 9 == b / 9 && c / 9 == d / 9 // Reverse case.
				&& b % 9 == c % 9
				&& d % 9 == e % 9 && e % 9 == f % 9
				|| e / 9 == f / 9 && c / 9 == d / 9
				&& d % 9 == e % 9
				&& b % 9 == c % 9 && a % 9 == b % 9;
		}
	}

	/// <summary>
	/// Indicates whether the chain is Local-Wing (<c>x = (x - z) = (z - y) = y</c>).
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private bool IsLocalWing
	{
		get
		{
			if (FlatComplexity != 8)
			{
				return false;
			}

			var chain = Target.WholeChain;
			var (a, _) = chain[1];
			var (b, _) = chain[2];
			var (c, _) = chain[3];
			var (d, _) = chain[4];
			var (e, _) = chain[5];
			var (f, _) = chain[6];

			return b / 9 == c / 9 && d / 9 == e / 9
				&& a % 9 == b % 9 && c % 9 == d % 9 && e % 9 == f % 9;
		}
	}

	[FormatItem]
	private string ChainStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new LinkCollection(from pair in Views[0].Links! select pair.Link).ToString();
	}
}
