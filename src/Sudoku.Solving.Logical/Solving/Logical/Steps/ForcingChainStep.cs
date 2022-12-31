namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Alternating Inference Chain</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Target">Indicates the target of the chain. This value can be used for constructing a whole chain.</param>
/// <param name="IsX"><inheritdoc/></param>
/// <param name="IsY"><inheritdoc/></param>
internal sealed record ForcingChainStep(ConclusionList Conclusions, Potential Target, bool IsX, bool IsY) :
	ChainingStep(Conclusions, IsX, IsY, false, false, false, 0)
{
	/// <inheritdoc/>
	public override int FlatComplexity => AncestorsCountOf(Target);

	/// <inheritdoc/>
	public override int SortKey => (IsX, IsY) switch { (true, true) => 4, (_, true) => 3, _ => 2 };

	/// <inheritdoc/>
	public override decimal Difficulty => (IsX, IsY) switch { (true, true) => 5.0M, _ => 4.6M } + LengthDifficulty;

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> (IsX, IsY) switch { (true, true) => Technique.AlternatingInferenceChain, (_, true) => Technique.YChain, _ => Technique.XChain };

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	protected override int FlatViewsCount => 1;

	/// <inheritdoc/>
	protected override Potential Result => Target;


	/// <inheritdoc/>
	protected internal override List<Potential> GetChainsTargets() => new() { Target };

	/// <inheritdoc/>
	protected override Potential GetChainTargetAt(int viewIndex) => Target;

	/// <inheritdoc/>
	protected override Candidates GetGreenPotentials(int viewIndex)
	{
		if (viewIndex >= FlatViewsCount)
		{
			return GetNestedGreenPotentials(viewIndex);
		}

		var result = GetColorCandidates(true);
		if (!Target.IsOn)
		{
			result.Remove(Target.Candidate);
		}

		return result;
	}

	/// <inheritdoc/>
	protected override Candidates GetRedPotentials(int viewIndex)
	{
		if (viewIndex >= FlatViewsCount)
		{
			return GetNestedRedPotentials(viewIndex);
		}

		var result = GetColorCandidates(false);
		if (Target.IsOn)
		{
			result.Remove(Target.Candidate);
		}

		return result;
	}

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex)
	{
		if (viewIndex >= FlatViewsCount)
		{
			return GetNestedLinks(viewIndex);
		}

		return GetLinks(Target);
	}

	/// <summary>
	/// Gets all colored candidates with the specified state.
	/// </summary>
	/// <param name="state">The state of the candidate you want to color.</param>
	/// <returns>All colored candidates with a same state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Candidates GetColorCandidates(bool state) => GetColorCandidates(Target, state, state);
}
