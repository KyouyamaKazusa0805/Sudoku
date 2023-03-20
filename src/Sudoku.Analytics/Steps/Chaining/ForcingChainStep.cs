namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Alternating Inference Chain</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
public sealed class ForcingChainStep(Conclusion[] conclusions, View[]? views, ChainNode target, bool isX, bool isY) :
	ChainingStep(conclusions, views, isX, isY)
{
	internal ForcingChainStep(Conclusion[] conclusions, ChainNode target, bool isX, bool isY) :
		this(conclusions, null!, target, isX, isY)
	{
	}

	internal ForcingChainStep(ForcingChainStep @base, View[]? views) : this(@base.Conclusions, views, @base.Target, @base.IsX, @base.IsY)
	{
	}


	/// <summary>
	/// Indicates the target of the chain. This value can be used for constructing a whole chain.
	/// </summary>
	public ChainNode Target { get; } = target;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CandStr, OnOffStr } }, { "zh", new[] { CandStr, OnOffStrZhCn } } };

	private string CandStr => RxCyNotation.ToCandidateString(Target.Candidate);

	private string OnOffStr => Target.IsOn.ToString().ToLower();

	private string OnOffStrZhCn => (Target.IsOn ? R["TrueKeyword"] : R["FalseKeyword"])!;


	/// <inheritdoc/>
	protected override CandidateMap GetGreenPotentials(int viewIndex)
	{
		if (viewIndex >= FlatViewsCount)
		{
			return GetNestedGreenPotentials(viewIndex);
		}

		var result = GetColorCandidates(true);
		if (Target is var (cand, isOn) && !isOn)
		{
			result.Remove(cand);
		}

		return result;
	}

	/// <inheritdoc/>
	protected override CandidateMap GetRedPotentials(int viewIndex)
	{
		if (viewIndex >= FlatViewsCount)
		{
			return GetNestedRedPotentials(viewIndex);
		}

		var result = GetColorCandidates(false);
		if (Target is var (cand, isOn) && isOn)
		{
			result.Remove(cand);
		}

		return result;
	}

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedLinks(viewIndex) : GetLinks(Target);

	/// <summary>
	/// Gets all colored candidates with the specified state.
	/// </summary>
	/// <param name="state">The state of the candidate you want to color.</param>
	/// <returns>All colored candidates with a same state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private CandidateMap GetColorCandidates(bool state) => GetColorCandidates(Target, state, state);
}
