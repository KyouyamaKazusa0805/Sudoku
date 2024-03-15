namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chain</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="isX">Indicates whether the chain supports for X chaining rule. "X chaining rule" aims to same-digit strong inferences.</param>
/// <param name="isY">Indicates whether the chain supports for Y chaining rule. "Y chaining rule" aims to same-cell inferences.</param>
/// <param name="isMultiple">Indicates whether the chain is a multiple forcing chains.</param>
/// <param name="isDynamic">Indicates whether the chain is a dynamic forcing chains.</param>
/// <param name="isNishio">Indicates whether the chain is a nishio forcing chains.</param>
/// <param name="dynamicNestingLevel">Indicates the dynamic nesting level.</param>
public abstract partial class ChainingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] bool isX = true,
	[PrimaryConstructorParameter] bool isY = true,
	[PrimaryConstructorParameter] bool isMultiple = false,
	[PrimaryConstructorParameter] bool isDynamic = false,
	[PrimaryConstructorParameter] bool isNishio = false,
	[PrimaryConstructorParameter] int dynamicNestingLevel = 0
) : Step(conclusions, views, options)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty
		=> this switch
		{
			{ DynamicNestingLevel: var l and >= 2 } => 9.5M + .5M * (l - 2),
			{ DynamicNestingLevel: var l and > 0 } => 8.5M + .5M * l,
			{ IsNishio: true } => 7.5M,
			{ IsDynamic: true } => 8.5M,
			{ IsMultiple: true } => 8.0M,
			(BidirectionalCycleStep or ForcingChainStep) and { IsX: true, IsY: true } => 5.0M,
			ForcingChainStep => 4.6M,
			BidirectionalCycleStep => 4.5M,
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("ChainMemberNotOverridden"))
		};

	/// <summary>
	/// Indicates the complexity of the chain.
	/// </summary>
	public int Complexity => FlatComplexity + NestedComplexity;

	/// <inheritdoc/>
	public sealed override Technique Code
		=> (this, IsX, IsY, IsDynamic, IsNishio) switch
		{
			(ForcingChainStep, true, true, _, _) => Technique.AlternatingInferenceChain,
			(ForcingChainStep, false, true, _, _) => Technique.YChain,
			(ForcingChainStep, true, false, _, _) => Technique.XChain,
			(BidirectionalCycleStep, true, true, _, _) => Technique.ContinuousNiceLoop,
			(BidirectionalCycleStep, false, true, _, _) => Technique.XyChain,
			(BidirectionalCycleStep, true, false, _, _) => Technique.FishyCycle,
			(CellForcingChainsStep, _, _, true, _) => Technique.DynamicCellForcingChains,
			(CellForcingChainsStep, _, _, false, _) => Technique.CellForcingChains,
			(RegionForcingChainsStep, _, _, true, _) => Technique.DynamicRegionForcingChains,
			(RegionForcingChainsStep, _, _, false, _) => Technique.RegionForcingChains,
			(BinaryForcingChainsStep, _, _, _, true) => Technique.NishioForcingChains,
			(BinaryForcingChainsStep { IsAbsurd: true }, _, _, _, false) => Technique.DynamicContradictionForcingChains,
			(BinaryForcingChainsStep, _, _, _, _) => Technique.DynamicDoubleForcingChains,
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("ChainMemberNotOverridden"))
		};

	/// <inheritdoc/>
	public sealed override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.Length, LengthDifficulty)];

	/// <summary>
	/// Indicates an <see cref="int"/> value indicating the ordering priority of the chain. Greater is heavier.
	/// </summary>
	internal int SortKey
		=> this switch
		{
			BinaryForcingChainsStep { IsAbsurd: false } => 1,
			ForcingChainStep or BidirectionalCycleStep => (IsX, IsY) switch { (true, true) => 4, (_, true) => 3, _ => 2 },
			CellForcingChainsStep => 5,
			RegionForcingChainsStep => 6,
			BinaryForcingChainsStep => 7
		};

	/// <summary>
	/// Indicates all possible targets that is used for checking the whole branches of the chain.
	/// </summary>
	protected internal ChainNode[] ChainsTargets
		=> this switch
		{
			ForcingChainStep { Target: var target } => [target],
			BidirectionalCycleStep { DestinationOn: var target } => [target],
			CellForcingChainsStep { Chains.Potentials: var targets } => [.. targets],
			RegionForcingChainsStep { Chains.Potentials: var targets } => [.. targets],
			BinaryForcingChainsStep { FromOnPotential: var on, FromOffPotential: var off } => [on, off],
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("ChainMemberNotOverridden"))
		};

	/// <summary>
	/// Returns how many views the current step will be used.
	/// </summary>
	protected int FlatViewsCount
		=> this switch
		{
			ForcingChainStep or BidirectionalCycleStep => 1,
			BinaryForcingChainsStep => 2,
			CellForcingChainsStep { Chains.Count: var count } => count,
			RegionForcingChainsStep { Chains.Count: var count } => count,
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("ChainMemberNotOverridden"))
		};

	/// <summary>
	/// Indicates the total number of views the step will be displayed.
	/// </summary>
	protected int ViewsCount => FlatViewsCount + NestedViewsCount;

	/// <summary>
	/// Indicates the result node.
	/// </summary>
	protected ChainNode? Result
		=> this switch
		{
			ForcingChainStep { Target: var target } => target,
			BidirectionalCycleStep => null,
			BinaryForcingChainsStep { SourcePotential: var (candidate, isOn), IsNishio: true } => new(candidate, !isOn),
			BinaryForcingChainsStep { SourcePotential: var (candidate, isOn), IsAbsurd: true } => new(candidate, !isOn),
			BinaryForcingChainsStep { FromOnPotential: var on } => on,
			CellForcingChainsStep { Chains.Potentials: [var branchedStart, ..] } => branchedStart,
			RegionForcingChainsStep { Chains.Potentials: [var branchedStart, ..] } => branchedStart,
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("ChainMemberNotOverridden"))
		};

	/// <summary>
	/// Indicates the difficulty rating of the current step, which binds with length factor.
	/// </summary>
	private decimal LengthDifficulty => StepRatingHelper.GetExtraDifficultyByLength(Complexity - 2);

	/// <summary>
	/// Indicates the complexity of the chain. The complexity value generally indicates the total length of all branches in a chain.
	/// </summary>
	private int FlatComplexity
		=> this switch
		{
			ForcingChainStep { Target: var target } => AncestorsCountOf(target),
			BidirectionalCycleStep { DestinationOn: var target } => AncestorsCountOf(target),
			BinaryForcingChainsStep { FromOnPotential: var on, FromOffPotential: var off } => AncestorsCountOf(on) + AncestorsCountOf(off),
			CellForcingChainsStep { Chains.Potentials: var branchedStarts } => branchedStarts.Sum(AncestorsCountOf),
			RegionForcingChainsStep { Chains.Potentials: var branchedStarts } => branchedStarts.Sum(AncestorsCountOf),
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("ChainMemberNotOverridden"))
		};

	/// <summary>
	/// Indicates the nested complexity of the chain. This property is useful on checking nesting chains.
	/// </summary>
	private int NestedComplexity
	{
		get
		{
			var result = 0;
			var processed = new HashSet<ChainingStep>(ChainStepEqualityComparer);
			foreach (var target in ChainsTargets)
			{
				foreach (var p in target.FullChainPotentials)
				{
					if (p.NestedChainDetails is { } nested && !processed.Contains(nested))
					{
						result += nested.Complexity;
						processed.Add(nested);
					}
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Returns the number of nested views.
	/// </summary>
	private int NestedViewsCount
	{
		get
		{
			var result = 0;
			var processed = new HashSet<ChainingStep>(ChainStepEqualityComparer);
			foreach (var target in ChainsTargets)
			{
				foreach (var p in target.FullChainPotentials)
				{
					if (p.NestedChainDetails is { } nested && !processed.Contains(nested))
					{
						result += nested.ViewsCount;
						processed.Add(nested);
					}
				}
			}

			return result;
		}
	}


	/// <summary>
	/// Represents an equality comparer of <see cref="ChainingStep"/> instances.
	/// </summary>
	private static IEqualityComparer<ChainingStep> ChainStepEqualityComparer
		=> ValueComparison.Create<ChainingStep?>(
			static (left, right) =>
			{
				return (left, right) switch
				{
					(null, null) => true,
					({ ChainsTargets: var x }, { ChainsTargets: var y }) => x.SequenceEquals(y) && branchEquals(x, y),
					_ => false
				};


				static bool branchEquals(scoped ReadOnlySpan<ChainNode> a, scoped ReadOnlySpan<ChainNode> b)
				{
					scoped var i1 = a.GetEnumerator();
					scoped var i2 = b.GetEnumerator();
					while (i1.MoveNext() && i2.MoveNext())
					{
						if (!i1.Current.FullChainPotentials.SequenceEquals(i2.Current.FullChainPotentials))
						{
							return false;
						}
					}

					return true;
				}
			},
			static ([DisallowNull] obj) =>
			{
				var result = 0;
				foreach (var target in obj.ChainsTargets)
				{
					foreach (var p in target.FullChainPotentials)
					{
						result ^= p.GetHashCode();
					}
				}

				return result;
			}
		);


	/// <inheritdoc/>
	public override int CompareTo(Step? other)
		=> other is ChainingStep comparer
			? Math.Sign(Difficulty - comparer.Difficulty) is var r1 and not 0
				? r1
				: Math.Sign(Complexity - comparer.Complexity) is var r2 and not 0
					? r2
					: Math.Sign(SortKey - comparer.SortKey) is var r3 and not 0 ? r3 : 0
			: 1;

	/// <inheritdoc/>
	public override string GetName(CultureInfo? culture = null)
	{
		culture ??= ResultCurrentCulture;
		return DynamicNestingLevel switch { 0 => prefixWithoutLevel(), var l => $"{prefixWithoutLevel()}{nestedSuffix(l)}" };


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string space() => CultureInfo.CurrentCulture.Name switch { ['Z' or 'z', 'H' or 'h', ..] => string.Empty, _ => " " };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		string dynamicKeyword() => $"{ResourceDictionary.Get("DynamicKeyword", culture)}{space()}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		string nestedSuffix(int level)
			=> level switch
			{
				1 => ResourceDictionary.Get("NestedSuffix_Level1", culture),
				2 => ResourceDictionary.Get("NestedSuffix_Level2", culture),
				3 => ResourceDictionary.Get("NestedSuffix_Level3", culture),
				4 => ResourceDictionary.Get("NestedSuffix_Level4", culture),
				>= 5 => string.Format(ResourceDictionary.Get("NestedSuffix_Level5OrGreater", culture), nestedSuffix(level - 3)),
				_ => string.Empty
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		string prefixWithoutLevel()
			=> this switch
			{
				ForcingChainStep or BidirectionalCycleStep => ResourceDictionary.Get("NormalChains", culture),
				CellForcingChainsStep { IsDynamic: false } => ResourceDictionary.Get("CellChains", culture),
				CellForcingChainsStep { IsDynamic: true } => $"{dynamicKeyword()}{ResourceDictionary.Get("CellChains", culture)}",
				RegionForcingChainsStep { IsDynamic: false } => ResourceDictionary.Get("HouseChains", culture),
				RegionForcingChainsStep { IsDynamic: true } => $"{dynamicKeyword()}{ResourceDictionary.Get("HouseChains", culture)}",
				BinaryForcingChainsStep { IsNishio: true } => ResourceDictionary.Get("NishioChains", culture),
				BinaryForcingChainsStep { IsAbsurd: true } => ResourceDictionary.Get("AbsurdChains", culture),
				BinaryForcingChainsStep => ResourceDictionary.Get("DoubleChains", culture)
			};
	}

	/// <summary>
	/// Gets parent rules. This method can only be used on advanced chain relations.
	/// </summary>
	/// <param name="initialGrid">The initial grid.</param>
	/// <param name="currentGrid">The current grid.</param>
	/// <returns>All found potentials.</returns>
	internal List<ChainNode> GetRuleParents(scoped ref readonly Grid initialGrid, scoped ref readonly Grid currentGrid)
	{
		var result = new List<ChainNode>();

		// Warning: Iterate on each chain target separately.
		// Reason: they may be equal according to equals() (same candidate), but they may have different parents !
		foreach (var target in ChainsTargets)
		{
			// Iterate on chain targets.
			CollectRuleParents(in initialGrid, in currentGrid, result, target);
		}

		return result;
	}

	/// <summary>
	/// To create views via the specified values.
	/// </summary>
	/// <param name="grid">The grid used.</param>
	/// <returns>The values.</returns>
	protected internal virtual View[] CreateViews(scoped ref readonly Grid grid)
	{
		var globalView = new View();
		var result = new View[ViewsCount];
		var i = 0;
		for (; i < FlatViewsCount; i++)
		{
			var view = new View();
			foreach (var candidate in GetOffPotentials(i))
			{
				var node = new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate);
				view.Add(node);
				globalView.Add(node);
			}
			foreach (var candidate in GetOnPotentials(i))
			{
				var node = new CandidateViewNode(ColorIdentifier.Normal, candidate);
				view.Add(node);
				globalView.Add(node);
			}

			var links = GetLinks(i);
			scoped var listOfNodes = links.AsReadOnlySpan();
			view.AddRange(listOfNodes);
			globalView.AddRange(listOfNodes);

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

		return [globalView, .. result];
	}

	/// <summary><b><i>
	/// This method will be implemented later.
	/// </i></b></summary>
	protected void CollectRuleParents(scoped ref readonly Grid initialGrid, scoped ref readonly Grid currentGrid, List<ChainNode> result, ChainNode target)
	{
		return;
	}

	/// <summary>
	/// Indicates the source potential from the specified target. This method can only be used for finding AICs.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <returns>The source potential.</returns>
	protected ChainNode SourcePotentialOf(ChainNode target)
	{
		var result = target;
		while (result.Parents.Count > 0)
		{
			result = result.Parents[0];
		}

		return result;
	}

	/// <summary>
	/// Try to fetch all candidates that will be colored in green in nested chains.
	/// </summary>
	/// <param name="nestedViewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected CandidateMap GetNestedOnPotentials(int nestedViewIndex)
	{
		nestedViewIndex -= FlatViewsCount;

		var (step, viewIndex) = GetNestedChain(nestedViewIndex);
		return step.GetOnPotentials(viewIndex);
	}

	/// <summary>
	/// Try to fetch all candidates that will be colored in red in nested chains.
	/// </summary>
	/// <param name="nestedViewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected CandidateMap GetNestedOffPotentials(int nestedViewIndex)
	{
		nestedViewIndex -= FlatViewsCount;

		var (step, viewIndex) = GetNestedChain(nestedViewIndex);
		return step.GetOffPotentials(viewIndex);
	}

	/// <summary>
	/// Try to fetch all candidates to be colored as state 1: the candidate is "on".
	/// </summary>
	/// <param name="viewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected abstract CandidateMap GetOnPotentials(int viewIndex);

	/// <summary>
	/// Try to fetch all candidates to be colored as state 2: the candidate is "off".
	/// </summary>
	/// <param name="viewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected abstract CandidateMap GetOffPotentials(int viewIndex);

	/// <summary>
	/// Try to fetch all candidates to be colored as state 3: the candidate is partially "off";
	/// they will be "off" in this view, but "on" in other views if used.
	/// </summary>
	/// <param name="grid">The grid as a candidate reference.</param>
	/// <param name="viewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected CandidateMap GetPartiallyOffPotentials(scoped ref readonly Grid grid, int viewIndex)
	{
		var result = (CandidateMap)[];

		viewIndex -= FlatViewsCount;
		if (viewIndex >= 0)
		{
			// Create the grid deduced from the container (or "main") chain.
			var nestedGrid = grid;

			var (step, nestedViewNum) = GetNestedChain(viewIndex);
			var target = GetContainerTarget(step);
			foreach (var (cell, digit, isOn) in target.FullChainPotentials)
			{
				if (!isOn)
				{
					// Remove deductions of the container chain.
					nestedGrid[cell] &= (Mask)(1 << digit);
				}
			}

			// Use the rule's parent collector.
			var blues = new List<ChainNode>();
			var nestedTarget = step.GetChainTargetAt(nestedViewNum);
			step.CollectRuleParents(in grid, in nestedGrid, blues, nestedTarget);

			foreach (var (candidate, _) in blues)
			{
				result.Add(candidate);
			}
		}

		return result;
	}

	/// <summary>
	/// Try to fetch all <see cref="LinkViewNode"/> instances of the specified view index.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	/// <returns>All <see cref="LinkViewNode"/>.</returns>
	protected abstract List<LinkViewNode> GetLinks(int viewIndex);

	/// <summary>
	/// Try to fetch all nested <see cref="LinkViewNode"/> instances of the specified view.
	/// </summary>
	/// <param name="nestedViewIndex">The specified index of the view.</param>
	/// <returns>All <see cref="LinkViewNode"/> instance in this view.</returns>
	protected List<LinkViewNode> GetNestedLinks(int nestedViewIndex)
	{
		nestedViewIndex -= FlatViewsCount;

		var (step, viewIndex) = GetNestedChain(nestedViewIndex);
		return step.GetLinks(viewIndex);
	}

	/// <summary>
	/// Try to get the target node of a chain, displayed in the view at the specified index.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	/// <returns>The <see cref="ChainNode"/> result.</returns>
	private ChainNode GetChainTargetAt(int viewIndex)
		=> this switch
		{
			ForcingChainStep { Target: var target } => target,
			BidirectionalCycleStep { DestinationOn: var target } => target,
			BinaryForcingChainsStep { FromOnPotential: var on, FromOffPotential: var off } => viewIndex switch { 0 => on, 1 => off },
			CellForcingChainsStep { Chains.Potentials: var targets } => targets[viewIndex],
			RegionForcingChainsStep { Chains.Potentials: var targets } => targets[viewIndex],
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("ChainMemberNotOverridden"))
		};

	/// <summary>
	/// Try to get container target potential via the specified step.
	/// </summary>
	/// <param name="step">The step instance.</param>
	/// <returns>The container target.</returns>
	private ChainNode GetContainerTarget(ChainingStep step)
	{
		foreach (var target in ChainsTargets)
		{
			foreach (var p in target.FullChainPotentials)
			{
				if (ReferenceEquals(p.NestedChainDetails, step))
				{
					return p;
				}
			}
		}

		return default;
	}

	/// <summary>
	/// Try to get nested chains, represented as <see cref="ChainingStep"/> instances.
	/// </summary>
	/// <returns>A list of <see cref="ChainingStep"/> instances.</returns>
	private List<ChainingStep> GetNestedChains()
	{
		var result = new List<ChainingStep>();
		var processed = new HashSet<ChainingStep>(ChainStepEqualityComparer);
		foreach (var target in ChainsTargets)
		{
			foreach (var p in target.FullChainPotentials)
			{
				if (p.NestedChainDetails is { } nested && !processed.Contains(nested))
				{
					result.Add(nested);
					processed.Add(nested);
				}
			}
		}

		// Recurse (in case there is more than one level of nesting).
		foreach (var chain in new List<ChainingStep>(result))
		{
			result.AddRange(chain.GetNestedChains());
		}

		return result;
	}

	/// <summary>
	/// Try to get all nested chains, represented as a pair of values:
	/// <list type="number">
	/// <item>
	/// <term>First</term>
	/// <description>A <see cref="ChainingStep"/> instance.</description>
	/// </item>
	/// <item>
	/// <term>Second</term>
	/// <description>An index of the step to be displayed.</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <param name="nestedViewIndex">The nested view index.</param>
	/// <returns>A pair of values.</returns>
	private (ChainingStep Step, int ViewIndex) GetNestedChain(int nestedViewIndex)
	{
		var processed = new HashSet<ChainingStep>(ChainStepEqualityComparer);
		foreach (var target in ChainsTargets)
		{
			foreach (var p in target.FullChainPotentials)
			{
				if (p.NestedChainDetails is { } nested && !processed.Contains(nested))
				{
					processed.Add(nested);
					var localCount = nested.ViewsCount;
					if (localCount > nestedViewIndex)
					{
						return (nested, nestedViewIndex);
					}

					nestedViewIndex -= localCount;
				}
			}
		}

		return default;
	}


	/// <summary>
	/// Try to get all ancestors from the specified node.
	/// </summary>
	/// <param name="child">The specified node.</param>
	/// <returns>The total number of all found ancestors.</returns>
	protected internal static int AncestorsCountOf(ChainNode child)
	{
		var ancestors = new NodeSet();
		var todo = new List<ChainNode> { child };
		while (todo.Count > 0)
		{
			var next = new List<ChainNode>();
			foreach (var p in todo)
			{
				if (ancestors.Add(p))
				{
					next.AddRange(p.Parents);
				}
			}

			todo = next;
		}

		return ancestors.Count;
	}

	/// <summary>
	/// Try to fetch all colored candidates with specified state.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <param name="state">The state of the node to be colored.</param>
	/// <param name="skipTarget">Indicates whether we should skip the target node.</param>
	/// <returns>All found candidates.</returns>
	protected internal static CandidateMap GetColorCandidates(ChainNode target, bool state, bool skipTarget)
	{
		var result = (CandidateMap)[];
		foreach (var p in target.FullChainPotentials)
		{
			var (cell, digit, isOn) = p;
			if (isOn == state || state && (p != target || !skipTarget))
			{
				result.Add(cell * 9 + digit);
			}
		}

		return result;
	}

	/// <summary>
	/// Try to fetch all <see cref="LinkViewNode"/> instances of the branch from the specified target,
	/// specified as a <see cref="ChainNode"/> instance.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <returns>All <see cref="LinkViewNode"/> displayed in this branch.</returns>
	protected internal static List<LinkViewNode> GetLinks(ChainNode target)
	{
		var result = new List<LinkViewNode>();
		foreach (var p in target.FullChainPotentials)
		{
			if (p is not (var pCell, var pDigit, var pIsOn) { Parents: { Count: <= 6 } pParents })
			{
				continue;
			}

			foreach (var (prCell, prDigit, prIsOn) in pParents)
			{
				result.Add(
					new(
						ColorIdentifier.Normal,
						new(prDigit, [prCell]),
						new(pDigit, [pCell]),
						(prIsOn, pIsOn) switch
						{
							(false, true) => Inference.Strong,
							(true, false) => Inference.Weak,
							(true, true) => Inference.StrongGeneralized,
							_ => Inference.WeakGeneralized
						}
					)
				);
			}
		}

		return result;
	}
}
