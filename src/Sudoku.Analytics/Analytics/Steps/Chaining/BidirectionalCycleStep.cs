using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bidirectional Cycle</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="destinationOn">Indicates the destination node that is "on" state.</param>
/// <param name="isX"><inheritdoc/></param>
/// <param name="isY"><inheritdoc/></param>
public sealed partial class BidirectionalCycleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[DataMember] ChainNode destinationOn,
	bool isX,
	bool isY
) : ChainingStep(conclusions, views, options, isX, isY)
{
	internal BidirectionalCycleStep(Conclusion[] conclusions, StepSearcherOptions options, ChainNode destinationOn, bool isX, bool isY) :
		this(conclusions, null!, options, destinationOn, isX, isY)
	{
	}

	internal BidirectionalCycleStep(BidirectionalCycleStep @base, View[]? views) :
		this(@base.Conclusions, views, @base.Options, @base.DestinationOn, @base.IsX, @base.IsY)
	{
	}


	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts => [new(EnglishLanguage, [CandsStr]), new(ChineseLanguage, [CandsStr])];

	private string CandsStr => Options.Converter.CandidateConverter([.. from element in Conclusions select element.Candidate]);


	/// <inheritdoc/>
	protected internal override View[] CreateViews(scoped ref readonly Grid grid)
	{
		var result = new View[ViewsCount];
		var i = 0;
		for (; i < FlatViewsCount; i++)
		{
			var view = new View();

			GetOffPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, candidate)));
			GetOnPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(WellKnownColorIdentifier.Normal, candidate)));
			view.AddRange(GetLinks(i));

			result[i] = view;
		}
		for (; i < ViewsCount; i++)
		{
			var view = new View();
			GetNestedOnPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(WellKnownColorIdentifier.Normal, candidate)));
			GetNestedOffPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, candidate)));
			GetPartiallyOffPotentials(in grid, i).ForEach(candidate => view.Add(new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, candidate)));
			view.AddRange(GetNestedLinks(i));

			result[i] = view;
		}

		return result;
	}

	/// <inheritdoc/>
	protected override CandidateMap GetOnPotentials(int viewIndex) => GetColorCandidates(DestinationOn, true, false);

	/// <inheritdoc/>
	protected override CandidateMap GetOffPotentials(int viewIndex) => GetColorCandidates(DestinationOn, false, true);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex) => GetLinks(DestinationOn);
}
