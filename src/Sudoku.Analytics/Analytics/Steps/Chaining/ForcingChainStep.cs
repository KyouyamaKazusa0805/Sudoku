namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Alternating Inference Chain</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="target">Indicates the target of the chain. This value can be used for constructing a whole chain.</param>
/// <param name="isX"><inheritdoc/></param>
/// <param name="isY"><inheritdoc/></param>
public sealed partial class ForcingChainStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[RecordParameter] ChainNode target,
	bool isX,
	bool isY
) : ChainingStep(conclusions, views, options, isX, isY)
{
	internal ForcingChainStep(Conclusion[] conclusions, StepSearcherOptions options, ChainNode target, bool isX, bool isY) :
		this(conclusions, null!, options, target, isX, isY)
	{
	}

	internal ForcingChainStep(ForcingChainStep @base, View[]? views) :
		this(@base.Conclusions, views, @base.Options, @base.Target, @base.IsX, @base.IsY)
	{
	}


	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CandStr, OnOffStr]), new(ChineseLanguage, [CandStr, OnOffStrZhCn])];

	private string CandStr => Options.Converter.CandidateConverter([Target.Candidate]);

	private string OnOffStr => Target.IsOn.ToString().ToLower();

	private string OnOffStrZhCn => ResourceDictionary.Get(Target.IsOn ? "TrueKeyword" : "FalseKeyword", new(2052))!;


	/// <inheritdoc/>
	protected internal override View[] CreateViews(scoped ref readonly Grid grid)
	{
		var result = new View[ViewsCount];
		var i = 0;
		for (; i < FlatViewsCount; i++)
		{
			var view = new View();

			GetOffPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate)));
			GetOnPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(ColorIdentifier.Normal, candidate)));
			view.AddRange(GetLinks(i).AsReadOnlySpan());

			result[i] = view;
		}
		for (; i < ViewsCount; i++)
		{
			var view = new View();
			GetNestedOnPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(ColorIdentifier.Normal, candidate)));
			GetNestedOffPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate)));
			GetPartiallyOffPotentials(in grid, i).ForEach(candidate => view.Add(new CandidateViewNode(ColorIdentifier.Auxiliary2, candidate)));
			view.AddRange(GetNestedLinks(i).AsReadOnlySpan());

			result[i] = view;
		}

		return result;
	}

	/// <inheritdoc/>
	protected override CandidateMap GetOnPotentials(int viewIndex)
	{
		if (viewIndex >= FlatViewsCount)
		{
			return GetNestedOnPotentials(viewIndex);
		}

		var result = GetColorCandidates(true);
		if (Target is (var cand, false))
		{
			result.Remove(cand);
		}

		return result;
	}

	/// <inheritdoc/>
	protected override CandidateMap GetOffPotentials(int viewIndex)
	{
		if (viewIndex >= FlatViewsCount)
		{
			return GetNestedOffPotentials(viewIndex);
		}

		var result = GetColorCandidates(false);
		if (Target is (var cand, true))
		{
			result.Remove(cand);
		}

		return result;
	}

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex) => viewIndex >= FlatViewsCount ? GetNestedLinks(viewIndex) : GetLinks(Target);

	/// <summary>
	/// Gets all colored candidates with the specified state.
	/// </summary>
	/// <param name="state">The state of the candidate you want to color.</param>
	/// <returns>All colored candidates with a same state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private CandidateMap GetColorCandidates(bool state) => GetColorCandidates(Target, state, state);
}
