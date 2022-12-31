namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Alternating Inference Chain</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Target">Indicates the target of the chain. This value can be used for constructing a whole chain.</param>
/// <param name="IsX"><inheritdoc/></param>
/// <param name="IsY"><inheritdoc/></param>
internal sealed record ForcingChainStep(ConclusionList Conclusions, Potential Target, bool IsX, bool IsY) : ChainingStep(Conclusions, IsX, IsY)
{
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
