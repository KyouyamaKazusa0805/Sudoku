namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a binary forcing chains (contradiction forcing chains or double forcing chains).
/// </summary>
/// <param name="branch1">Indicates the first branch.</param>
/// <param name="branch2">Indicates the second branch.</param>
/// <param name="conclusion">Indicates the conclusion.</param>
/// <param name="isContradiction">Indicates whether the pattern is a contradiction forcing chains.</param>
public sealed partial class BinaryForcingChains(
	[Property] UnnamedChain branch1,
	[Property] UnnamedChain branch2,
	[Property] Conclusion conclusion,
	[Property] bool isContradiction
) :
	IForcingChains,
	IFormattable
{
	/// <inheritdoc/>
	public bool IsGrouped => false;

	/// <inheritdoc/>
	public bool IsStrictlyGrouped => false;

	/// <inheritdoc/>
	public int Complexity => BranchedComplexity.Sum();

	/// <inheritdoc/>
	public Mask DigitsMask
	{
		get
		{
			var result = (Mask)0;
			foreach (var value in Branch1)
			{
				result |= value.Map.Digits;
			}
			foreach (var value in Branch2)
			{
				result |= value.Map.Digits;
			}
			return result;
		}
	}

	/// <inheritdoc/>
	public ReadOnlySpan<int> BranchedComplexity => from v in Branches select v.Length;

	/// <inheritdoc/>
	ComponentType IComponent.Type => ComponentType.BinaryForcingChains;

	/// <inheritdoc/>
	StepConclusions IForcingChains.Conclusions => new SingletonArray<Conclusion>(Conclusion);

	/// <summary>
	/// Indicates the backing branches.
	/// </summary>
	private ReadOnlySpan<UnnamedChain> Branches => (UnnamedChain[])[Branch1, Branch2];


	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		return string.Join(", ", from branch in Branches select $"{branch.ToString(converter)}");
	}

	/// <inheritdoc/>
	public View[] GetViews(ref readonly Grid grid, Conclusion[] newConclusions, ChainingRuleCollection supportedRules)
	{
		var viewNodes = GetViewsCore(in grid, supportedRules, newConclusions);
		var result = new View[viewNodes.Length];
		for (var i = 0; i < viewNodes.Length; i++)
		{
			CandidateMap elimMap = [.. from conclusion in newConclusions select conclusion.Candidate];
			result[i] = [
				..
				from node in viewNodes[i]
				where node is not CandidateViewNode { Candidate: var c } || !elimMap.Contains(c)
				select node
			];
		}

		var (viewIndex, cachedAlsIndex) = (1, 0);
		foreach (var branch in Branches)
		{
			var context = new ChainingRuleViewNodeContext(in grid, branch, result[viewIndex++]) { CurrentAlmostLockedSetIndex = cachedAlsIndex };
			foreach (var supportedRule in supportedRules)
			{
				supportedRule.GetViewNodes(ref context);
				result[0].AddRange(context.ProducedViewNodes);
			}
			cachedAlsIndex = context.CurrentAlmostLockedSetIndex;
		}
		return result;
	}

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <summary>
	/// Represents a method that creates a list of views.
	/// </summary>
	/// <param name="grid">The target grid.</param>
	/// <param name="rules">The rules used.</param>
	/// <param name="newConclusions">The conclusions used.</param>
	/// <returns>A list of nodes.</returns>
	private ReadOnlySpan<ViewNode[]> GetViewsCore(ref readonly Grid grid, ChainingRuleCollection rules, Conclusion[] newConclusions)
	{
		var result = new ViewNode[3][];
		var i = 0;
		var globalView = new List<ViewNode>();
		foreach (var chain in Branches)
		{
			var subview = View.Empty;
			var j = 0;
			foreach (var node in chain)
			{
				var id = (++j & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
				foreach (var candidate in node.Map)
				{
					var currentViewNode = new CandidateViewNode(id, candidate);
					globalView.Add(currentViewNode);
					subview.Add(currentViewNode);
				}
			}

			j = 0;
			foreach (var link in chain.Links)
			{
				// Skip the link if there are >= 2 conclusions.
				if (newConclusions.Length >= 2 && j++ == 0)
				{
					continue;
				}

				var id = (++j & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
				var currentViewNode = new ChainLinkViewNode(id, link.FirstNode.Map, link.SecondNode.Map, link.IsStrong);
				globalView.Add(currentViewNode);
				subview.Add(currentViewNode);
			}
			result[++i] = [.. subview];
		}
		result[0] = [.. globalView];
		return result;
	}
}
