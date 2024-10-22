namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that can be used for binding as source, for a view unit.
/// </summary>
public sealed partial class ViewUnitBindableSource : DependencyObject, ICloneable, IDrawable
{
	/// <summary>
	/// Initializes a <see cref="ViewUnitBindableSource"/> instance.
	/// </summary>
	public ViewUnitBindableSource() : this(ReadOnlyMemory<Conclusion>.Empty, [])
	{
	}

	/// <summary>
	/// Initializes a <see cref="ViewUnitBindableSource"/> instance via the specified values.
	/// </summary>
	/// <param name="conclusions">The conclusions.</param>
	/// <param name="view">The view.</param>
	public ViewUnitBindableSource(ReadOnlyMemory<Conclusion> conclusions, View view) => (Conclusions, View) = (conclusions, view);


	/// <summary>
	/// Indicates the candidates as conclusions in a single <see cref="Step"/>.
	/// </summary>
	[DependencyProperty]
	public partial ReadOnlyMemory<Conclusion> Conclusions { get; set; }

	/// <summary>
	/// Indicates a view of highlight elements.
	/// </summary>
	[DependencyProperty]
	public partial View View { get; set; }

	/// <inheritdoc/>
	ReadOnlyMemory<View> IDrawable.Views => (View[])[View];


	/// <summary>
	/// Determine whether the collection contains the specified candidate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public bool CandidateContains(Candidate candidate)
	{
		foreach (var conclusion in Conclusions)
		{
			if (conclusion.Candidate == candidate)
			{
				return true;
			}
		}
		foreach (var viewNode in View.OfType<CandidateViewNode>())
		{
			if (viewNode.Candidate == candidate)
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc cref="ICloneable.Clone"/>
	public ViewUnitBindableSource Clone() => new() { Conclusions = Conclusions[..], View = View.Clone() };

	/// <inheritdoc/>
	object ICloneable.Clone() => Clone();


	/// <summary>
	/// Make subtraction to get delta items.
	/// </summary>
	public static ViewUnitBindableSourceDiff operator -(ViewUnitBindableSource? old, ViewUnitBindableSource? @new)
	{
		return (old, @new) switch
		{
			(null, null) => new() { Negatives = [], Positives = [] },
			(not null, null) => new() { Negatives = f(old), Positives = [] },
			(null, not null) => new() { Negatives = [], Positives = f(@new) },
			_ => g(old, @new)
		};


		static ReadOnlySpan<IDrawableItem> f(ViewUnitBindableSource value)
		{
			var result = new List<IDrawableItem>();
			result.AddRange(from conclusion in value.Conclusions.Span select (IDrawableItem)conclusion);
			result.AddRange(value.View);

			foreach (var node in value.View.OfType<ChainLinkViewNode>())
			{
				if (node.Start is { Count: > 1 } s)
				{
					result.Add(new GroupedNodeInfo(in s));
				}
				if (node.End is { Count: > 1 } e)
				{
					result.Add(new GroupedNodeInfo(in e));
				}
			}
			return result.AsSpan();
		}

		static ViewUnitBindableSourceDiff g(ViewUnitBindableSource left, ViewUnitBindableSource right)
		{
			var (positives, negatives) = (new List<IDrawableItem>(), new List<IDrawableItem>());
			getDeltaConclusions(
				left,
				right,
				out var negativeConclusions,
				out var positiveConclusions,
				out var negativeGroupedNodes,
				out var positiveGroupedNodes
			);
			negatives.AddRange(negativeConclusions);
			positives.AddRange(positiveConclusions);
			negatives.AddRange(from node in left.View.ExceptWith(right.View) select (IDrawableItem)node);
			positives.AddRange(from node in right.View.ExceptWith(left.View) select (IDrawableItem)node);
			negatives.AddRange(from node in negativeGroupedNodes select (IDrawableItem)new GroupedNodeInfo(node));
			positives.AddRange(from node in positiveGroupedNodes select (IDrawableItem)new GroupedNodeInfo(node));
			return new() { Negatives = negatives.AsSpan(), Positives = positives.AsSpan() };
		}

		static void getDeltaConclusions(
			ViewUnitBindableSource left,
			ViewUnitBindableSource right,
			out ReadOnlySpan<IDrawableItem> negativeConclusions,
			out ReadOnlySpan<IDrawableItem> positiveConclusions,
			out ReadOnlySpan<CandidateMap> negativeGroupedNodes,
			out ReadOnlySpan<CandidateMap> positiveGroupedNodes
		)
		{
			// To distinct conclusions, we should split into two steps:
			//   1) Find conclusions that will be used in both old and new collection (it won't be removed).
			//   2) Check the state on conclusion overlapping cases.
			// If and only if the conclusion will be appeared in both collections, and its state has been changed,
			// it will be updated from both negatives and positives; otherwise, don't change anything.
			var (positives, negatives) = (new List<IDrawableItem>(), new List<IDrawableItem>());
			handleConclusions();
			handlePassedThroughDiffers();
			handleGroupedNodes(out negativeGroupedNodes, out positiveGroupedNodes);
			negativeConclusions = negatives.AsSpan();
			positiveConclusions = positives.AsSpan();


			void handleConclusions()
			{
				(ConclusionSet l, ConclusionSet r) = ([.. left.Conclusions], [.. right.Conclusions]);
				foreach (var c in l & ~r)
				{
					negatives.Add(c);
				}
				foreach (var c in r & ~l)
				{
					positives.Add(c);
				}

				foreach (var conclusion in l & r)
				{
					var leftCase = new ConclusionInfo(conclusion, false);
					var rightCase = new ConclusionInfo(conclusion, false);
					foreach (var node in left.View.OfType<CandidateViewNode>())
					{
						if (node.Candidate == conclusion.Candidate)
						{
							leftCase = leftCase with { IsOverlapped = true };
							break;
						}
					}
					foreach (var node in right.View.OfType<CandidateViewNode>())
					{
						if (node.Candidate == conclusion.Candidate)
						{
							rightCase = rightCase with { IsOverlapped = true };
							break;
						}
					}

					if (leftCase != rightCase)
					{
						negatives.Add(conclusion);
						positives.Add(conclusion);
					}
				}
			}

			void handlePassedThroughDiffers()
			{
				var g = Application.Current.AsApp().MainSudokuPane?.MainGrid ?? throw new InvalidOperationException();
				var leftChainNodes = ReadOnlySpan<ILinkViewNode>.CastUp(left.View.OfType<ChainLinkViewNode>());
				var rightChainNodes = ReadOnlySpan<ILinkViewNode>.CastUp(right.View.OfType<ChainLinkViewNode>());
				foreach (var link in (left.View & right.View).OfType<ChainLinkViewNode>())
				{
					if (link.IsPassedThrough(leftChainNodes, left.View.OfType<CandidateViewNode>(), left.Conclusions.Span, g)
						^ link.IsPassedThrough(rightChainNodes, right.View.OfType<CandidateViewNode>(), right.Conclusions.Span, g))
					{
						negatives.Add(link);
						positives.Add(link);
					}
				}
			}

			void handleGroupedNodes(out ReadOnlySpan<CandidateMap> negativeGroupedNodes, out ReadOnlySpan<CandidateMap> positiveGroupedNodes)
			{
				var (leftInfo, rightInfo) = (new HashSet<CandidateMap>(), new HashSet<CandidateMap>());
				foreach (var leftLink in left.View.OfType<ChainLinkViewNode>())
				{
					if (leftLink.Start.Count >= 2)
					{
						leftInfo.Add(leftLink.Start);
					}
					if (leftLink.End.Count >= 2)
					{
						leftInfo.Add(leftLink.End);
					}
				}
				foreach (var rightLink in right.View.OfType<ChainLinkViewNode>())
				{
					if (rightLink.Start.Count >= 2)
					{
						rightInfo.Add(rightLink.Start);
					}
					if (rightLink.End.Count >= 2)
					{
						rightInfo.Add(rightLink.End);
					}
				}
				negativeGroupedNodes = leftInfo.Except(rightInfo).ToArray();
				positiveGroupedNodes = rightInfo.Except(leftInfo).ToArray();
			}
		}
	}
}

/// <summary>
/// Indicates information of a conclusion.
/// </summary>
/// <param name="Conclusion">Indicates the conclusion to be used.</param>
/// <param name="IsOverlapped">Indicates whether the conclusion is overlapped.</param>
file readonly record struct ConclusionInfo(Conclusion Conclusion, bool IsOverlapped);

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{

}
