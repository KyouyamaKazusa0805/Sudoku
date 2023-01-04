namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Chain</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="IsX">Indicates whether the chain is X-Chain.</param>
/// <param name="IsY">Indicates whether the chain is Y-Chain.</param>
/// <param name="IsMultiple">
/// <para><inheritdoc cref="ChainingStepSearcher.AllowMultiple" path="/summary"/></para>
/// <para><inheritdoc cref="ChainingStepSearcher.AllowMultiple" path="/remarks"/></para>
/// </param>
/// <param name="IsDynamic">
/// <para><inheritdoc cref="ChainingStepSearcher.AllowDynamic" path="/summary"/></para>
/// <para><inheritdoc cref="ChainingStepSearcher.AllowDynamic" path="/remarks"/></para>
/// </param>
/// <param name="IsNishio">
/// <inheritdoc cref="ChainingStepSearcher.AllowNishio" path="/summary"/>
/// </param>
/// <param name="DynamicNestingLevel">
/// <para><inheritdoc cref="ChainingStepSearcher.DynamicNestingLevel" path="/summary"/></para>
/// <para><inheritdoc cref="ChainingStepSearcher.DynamicNestingLevel" path="/remarks"/></para>
/// </param>
internal abstract record ChainingStep(
	ConclusionList Conclusions,
	bool IsX = true,
	bool IsY = true,
	bool IsMultiple = false,
	bool IsDynamic = false,
	bool IsNishio = false,
	int DynamicNestingLevel = 0
) : Step(Conclusions, ViewList.Empty), IChainLikeStep
{
	/// <summary>
	/// Defines a target type not supported message.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private const string TargetTypeNotSupportedMessage = "The target type of the chain is not supported. You should override this property for that type.";


	/// <inheritdoc/>
	public sealed override decimal Difficulty
		=> LengthDifficulty + this switch
		{
			{ DynamicNestingLevel: var l and >= 2 } => 9.5M + .5M * (l - 2),
			{ DynamicNestingLevel: var l and > 0 } => 8.5M + .5M * l,
			{ IsNishio: true } => 7.5M,
			{ IsDynamic: true } => 8.5M,
			{ IsMultiple: true } => 8.0M,
			(BidirectionalCycleStep or ForcingChainStep) and { IsX: true, IsY: true } => 5.0M,
			ForcingChainStep => 4.6M,
			BidirectionalCycleStep => 4.5M,
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
		};

	/// <inheritdoc/>
	public override string Name
	{
		get
		{
			return DynamicNestingLevel switch { 0 => prefixWithoutLevel(), var l => $"{prefixWithoutLevel()}{nestedSuffix(l)}" };


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string space() => CultureInfo.CurrentCulture.Name switch { ['Z' or 'z', 'H' or 'h', ..] => string.Empty, _ => " " };

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string dynamicKeyword() => $"{R["DynamicKeyword"]!}{space()}";

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string nestedSuffix(int level)
				=> level switch
				{
					1 => R["NestedSuffix_Level1"]!,
					2 => R["NestedSuffix_Level2"]!,
					3 => R["NestedSuffix_Level3"]!,
					4 => R["NestedSuffix_Level4"]!,
					>= 5 => string.Format(R["NestedSuffix_Level5OrGreater"]!, nestedSuffix(level - 3)),
					_ => string.Empty
				};

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			string prefixWithoutLevel()
				=> this switch
				{
					ForcingChainStep => R["NormalChains"]!,
					CellForcingChainsStep { IsDynamic: false } => R["CellChains"]!,
					CellForcingChainsStep { IsDynamic: true } => $"{dynamicKeyword()}{R["CellChains"]!}",
					RegionForcingChainsStep { IsDynamic: false } => R["HouseChains"]!,
					RegionForcingChainsStep { IsDynamic: true } => $"{dynamicKeyword()}{R["HouseChains"]!}",
					BinaryForcingChainsStep { IsNishio: true } => R["NishioChains"]!,
					BinaryForcingChainsStep { IsAbsurd: true } => R["AbsurdChains"]!,
					BinaryForcingChainsStep => R["DoubleChains"]!,
				};
		}
	}

	/// <summary>
	/// Indicates the complexity of the chain.
	/// </summary>
	public int Complexity => FlatComplexity + NestedComplexity;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode
		=> (this, IsX, IsY, IsDynamic, IsNishio) switch
		{
			(ForcingChainStep, true, true, _, _) => Technique.AlternatingInferenceChain,
			(ForcingChainStep, false, true, _, _) => Technique.YChain,
			(ForcingChainStep, false, false, _, _) => Technique.XChain,
			(BidirectionalCycleStep, true, true, _, _) => Technique.ContinuousNiceLoop,
			(BidirectionalCycleStep, false, true, _, _) => Technique.XyChain,
			(BidirectionalCycleStep, false, false, _, _) => Technique.FishyCycle,
			(CellForcingChainsStep, _, _, true, _) => Technique.DynamicCellForcingChains,
			(CellForcingChainsStep, _, _, false, _) => Technique.CellForcingChains,
			(RegionForcingChainsStep, _, _, true, _) => Technique.DynamicRegionForcingChains,
			(RegionForcingChainsStep, _, _, false, _) => Technique.RegionForcingChains,
			(BinaryForcingChainsStep, _, _, _, true) => Technique.NishioForcingChains,
			(BinaryForcingChainsStep { IsAbsurd: true }, _, _, _, false) => Technique.DynamicContradictionForcingChains,
			(BinaryForcingChainsStep, _, _, _, _) => Technique.DynamicDoubleForcingChains,
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
		};

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags
		=> this switch
		{
			ForcingChainStep or BidirectionalCycleStep => TechniqueTags.LongChaining,
			CellForcingChainsStep or RegionForcingChainsStep or BinaryForcingChainsStep => TechniqueTags.LongChaining | TechniqueTags.ForcingChains,
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
		};

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup
		=> this switch
		{
			ForcingChainStep or BidirectionalCycleStep => TechniqueGroup.AlternatingInferenceChain,
			_ => TechniqueGroup.ForcingChains
		};

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel
		=> this switch
		{
			ForcingChainStep or BidirectionalCycleStep => DifficultyLevel.Fiendish,
			CellForcingChainsStep or RegionForcingChainsStep or BinaryForcingChainsStep => DifficultyLevel.Nightmare,
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
		};

	/// <inheritdoc/>
	public sealed override Rarity Rarity
		=> this switch { { DynamicNestingLevel: >= 2 } => Rarity.OnlyForSpecialPuzzles, _ => Rarity.HardlyEver };

	/// <summary>
	/// Indicates all possible targets that is used for checking the whole branches of the chain.
	/// </summary>
	protected internal Potential[] ChainsTargets
		=> this switch
		{
			ForcingChainStep { Target: var target } => new[] { target },
			BidirectionalCycleStep { DestinationOn: var target } => new[] { target },
			CellForcingChainsStep { Chains.Potentials: var targets } => ((List<Potential>)targets).ToArray(),
			RegionForcingChainsStep { Chains.Potentials: var targets } => ((List<Potential>)targets).ToArray(),
			BinaryForcingChainsStep { FromOnPotential: var on, FromOffPotential: var off } => new[] { on, off },
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
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
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
		};

	/// <summary>
	/// Indicates the result node.
	/// </summary>
	protected Potential? Result
		=> this switch
		{
			ForcingChainStep { Target: var target } => target,
			BidirectionalCycleStep => null,
			BinaryForcingChainsStep { SourcePotential: var (candidate, isOn), IsNishio: true } => new(candidate, !isOn),
			BinaryForcingChainsStep { SourcePotential: var (candidate, isOn), IsAbsurd: true } => new(candidate, !isOn),
			BinaryForcingChainsStep { FromOnPotential: var on } => on,
			CellForcingChainsStep { Chains.Potentials: [var branchedStart, ..] } => branchedStart,
			RegionForcingChainsStep { Chains.Potentials: [var branchedStart, ..] } => branchedStart,
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
		};

	/// <summary>
	/// Indicates the difficulty rating of the current step, which binds with length factor.
	/// </summary>
	[DebuggerHidden]
	private decimal LengthDifficulty
	{
		get
		{
			var result = 0.0M;
#if true
			for (var (ceil, length, isOdd) = (4, Complexity - 2, false); length > ceil; result += .1M, isOdd = !isOdd)
			{
				ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
			}
#else
			var steps = new[] { 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192, 256, 384, 512, 768, 1024, 1536, 2048, 3072, 4096, 6144, 8192 };
			for (var (length, index) = (Complexity - 2, 0); index < Steps.Length && length > Steps[index]; result += .1M, index++) ;
#endif

			return result;
		}
	}

	/// <summary>
	/// Indicates the total number of views the step will be displayed.
	/// </summary>
	[DebuggerHidden]
	private int ViewsCount => FlatViewsCount + NestedViewsCount;

	/// <summary>
	/// Indicates the complexity of the chain. The complexity value generally indicates the total length of all branches in a chain.
	/// </summary>
	[DebuggerHidden]
	private int FlatComplexity
		=> this switch
		{
			ForcingChainStep { Target: var target } => AncestorsCountOf(target),
			BidirectionalCycleStep { DestinationOn: var target } => AncestorsCountOf(target),
			BinaryForcingChainsStep { FromOnPotential: var on, FromOffPotential: var off } => AncestorsCountOf(on) + AncestorsCountOf(off),
			CellForcingChainsStep { Chains.Potentials: var branchedStarts } => branchedStarts.Sum(AncestorsCountOf),
			RegionForcingChainsStep { Chains.Potentials: var branchedStarts } => branchedStarts.Sum(AncestorsCountOf),
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
		};

	/// <summary>
	/// Indicates an <see cref="int"/> value indicating the ordering priority of the chain. Greater is heavier.
	/// </summary>
	[DebuggerHidden]
	private int SortKey
		=> this switch
		{
			BinaryForcingChainsStep { IsAbsurd: false } => 1,
			ForcingChainStep or BidirectionalCycleStep => (IsX, IsY) switch { (true, true) => 4, (_, true) => 3, _ => 2 },
			CellForcingChainsStep => 5,
			RegionForcingChainsStep => 6,
			BinaryForcingChainsStep => 7
		};

	/// <summary>
	/// Indicates the nested complexity of the chain. This property is useful on checking nesting chains.
	/// </summary>
	[DebuggerHidden]
	private int NestedComplexity
	{
		get
		{
			var result = 0;
			var processed = new HashSet<ChainingStep>(EqualityComparer.Instance);
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
	[DebuggerHidden]
	private int NestedViewsCount
	{
		get
		{
			var result = 0;
			var processed = new HashSet<ChainingStep>(EqualityComparer.Instance);
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
	/// Gets parent rules. This method can only be used on advanced chain relations.
	/// </summary>
	/// <param name="initialGrid">The initial grid.</param>
	/// <param name="currentGrid">The current grid.</param>
	/// <returns>All found potentials.</returns>
	internal List<Potential> GetRuleParents(scoped in Grid initialGrid, scoped in Grid currentGrid)
	{
		var result = new List<Potential>();

		// Warning: Iterate on each chain target separately.
		// Reason: they may be equal according to equals() (same candidate), but they may have different parents !
		foreach (var target in ChainsTargets)
		{
			// Iterate on chain targets.
			CollectRuleParents(initialGrid, currentGrid, result, target);
		}

		return result;
	}

	/// <summary>
	/// To create views via the specified values.
	/// </summary>
	/// <param name="grid">The grid used.</param>
	/// <returns>The values.</returns>
	protected internal ViewList CreateViews(scoped in Grid grid)
	{
		var result = new View[ViewsCount];

		for (var i = 0; i < FlatViewsCount; i++)
		{
			var view = View.Empty;

			GetGreenPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(DisplayColorKind.Normal, candidate)));
			GetRedPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(DisplayColorKind.Auxiliary1, candidate)));
			view.AddRange(GetLinks(i));

			result[i] = view;
		}

		for (var i = FlatViewsCount; i < ViewsCount; i++)
		{
			var view = View.Empty;

			GetNestedGreenPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(DisplayColorKind.Normal, candidate)));
			GetNestedRedPotentials(i).ForEach(candidate => view.Add(new CandidateViewNode(DisplayColorKind.Auxiliary1, candidate)));
			GetBluePotentials(grid, i).ForEach(candidate => view.Add(new CandidateViewNode(DisplayColorKind.Auxiliary2, candidate)));
			view.AddRange(GetNestedLinks(i));

			result[i] = view;
		}

		return ImmutableArray.Create(result);
	}

	/// <summary><b><i>
	/// This method will be implemented later.
	/// </i></b></summary>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	protected void CollectRuleParents(scoped in Grid initialGrid, scoped in Grid currentGrid, List<Potential> result, Potential target)
	{
		return;
	}

	/// <summary>
	/// Try to get all ancestors from the specified node.
	/// </summary>
	/// <param name="child">The specified node.</param>
	/// <returns>The total number of all found ancestors.</returns>
	protected int AncestorsCountOf(Potential child)
	{
		var ancestors = new PotentialSet();
		var todo = new List<Potential> { child };
		while (todo.Count > 0)
		{
			var next = new List<Potential>();
			foreach (var p in todo)
			{
				if (!ancestors.Contains(p))
				{
					ancestors.Add(p);
					next.AddRange(p.Parents);
				}
			}

			todo = next;
		}

		return ancestors.Count;
	}

	/// <summary>
	/// Indicates the source potential from the specified target. This method can only be used for finding AICs.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <returns>The source potential.</returns>
	protected Potential SourcePotentialOf(Potential target)
	{
		var result = target;
		while (result.Parents.Count > 0)
		{
			result = result.Parents[0];
		}

		return result;
	}

	/// <summary>
	/// Try to fetch all colored candidates with specified state.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <param name="state">The state of the node to be colored.</param>
	/// <param name="skipTarget">Indicates whether we should skip the target node.</param>
	/// <returns>All found candidates.</returns>
	protected Candidates GetColorCandidates(Potential target, bool state, bool skipTarget)
	{
		var result = Candidates.Empty;
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
	/// Try to fetch all candidates that will be colored in green in nested chains.
	/// </summary>
	/// <param name="nestedViewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected Candidates GetNestedGreenPotentials(int nestedViewIndex)
	{
		nestedViewIndex -= FlatViewsCount;

		var (step, viewIndex) = GetNestedChain(nestedViewIndex);
		return step.GetGreenPotentials(viewIndex);
	}

	/// <summary>
	/// Try to fetch all candidates that will be colored in red in nested chains.
	/// </summary>
	/// <param name="nestedViewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected Candidates GetNestedRedPotentials(int nestedViewIndex)
	{
		nestedViewIndex -= FlatViewsCount;

		var (step, viewIndex) = GetNestedChain(nestedViewIndex);
		return step.GetRedPotentials(viewIndex);
	}

	/// <summary>
	/// Try to fetch all candidates to be colored in green (state 1: the candidate is "on").
	/// </summary>
	/// <param name="viewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected abstract Candidates GetGreenPotentials(int viewIndex);

	/// <summary>
	/// Try to fetch all candidates to be colored in red (state 2: the candidate is "off").
	/// </summary>
	/// <param name="viewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected abstract Candidates GetRedPotentials(int viewIndex);

	/// <summary>
	/// Try to fetch all candidates to be colored in blue
	/// (state 3: the candidate is partially "off"; they will be "off" in this view, but "on" in other views if used).
	/// </summary>
	/// <param name="grid">The grid as a candidate reference.</param>
	/// <param name="viewIndex">The specified index of the view.</param>
	/// <returns>All found candidates.</returns>
	protected Candidates GetBluePotentials(scoped in Grid grid, int viewIndex)
	{
		var result = Candidates.Empty;

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
					nestedGrid.GetMaskRef(cell) &= (short)(1 << digit);
				}
			}

			// Use the rule's parent collector.
			var blues = new List<Potential>();
			var nestedTarget = step.GetChainTargetAt(nestedViewNum);
			step.CollectRuleParents(grid, nestedGrid, blues, nestedTarget);

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
	/// Try to fetch all <see cref="LinkViewNode"/> instances of the branch from the specified target, specified as a <see cref="Potential"/> instance.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <returns>All <see cref="LinkViewNode"/> displayed in this branch.</returns>
	protected List<LinkViewNode> GetLinks(Potential target)
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
						DisplayColorKind.Normal,
						new(prDigit, prCell),
						new(pDigit, pCell),
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

	/// <summary>
	/// Try to fetch all nested <see cref="LinkViewNode"/> instances of the specified view.
	/// </summary>
	/// <param name="nestedViewNum">The specified index of the view.</param>
	/// <returns>All <see cref="LinkViewNode"/> instance in this view.</returns>
	protected List<LinkViewNode> GetNestedLinks(int nestedViewNum)
	{
		nestedViewNum -= FlatViewsCount;

		var (step, nestedViewIndex) = GetNestedChain(nestedViewNum);
		return step.GetLinks(nestedViewIndex);
	}

	/// <summary>
	/// Try to get the target node of a chain, displayed in the view at the specified index.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	/// <returns>The <see cref="Potential"/> result.</returns>
	private Potential GetChainTargetAt(int viewIndex)
		=> this switch
		{
			ForcingChainStep { Target: var target } => target,
			BidirectionalCycleStep { DestinationOn: var target } => target,
			BinaryForcingChainsStep { FromOnPotential: var on, FromOffPotential: var off } => viewIndex switch { 0 => on, 1 => off },
			CellForcingChainsStep { Chains.Potentials: var targets } => targets[viewIndex],
			RegionForcingChainsStep { Chains.Potentials: var targets } => targets[viewIndex],
			_ => throw new NotSupportedException(TargetTypeNotSupportedMessage)
		};

	/// <summary>
	/// Try to get container target potential via the specified step.
	/// </summary>
	/// <param name="step">The step instance.</param>
	/// <returns>The container target.</returns>
	private Potential GetContainerTarget(ChainingStep step)
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
	private IEnumerable<ChainingStep> GetNestedChains()
	{
		var result = new List<ChainingStep>();
		var processed = new HashSet<ChainingStep>(EqualityComparer.Instance);
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
		var processed = new HashSet<ChainingStep>(EqualityComparer.Instance);
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
	/// Compares two <see cref="ChainingStep"/> instance, and returns an <see cref="int"/>
	/// indicating which value is greater.
	/// </summary>
	/// <param name="left">The left-side value to be compared.</param>
	/// <param name="right">The right-side value to be compared.</param>
	/// <returns>An <see cref="int"/> value indicating which is greater.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compare(ChainingStep left, ChainingStep right)
		=> Sign(left.Difficulty - right.Difficulty) is var d and not 0
			? d
			: Sign(left.Complexity - right.Complexity) is var c and not 0
				? c
				: Sign(left.SortKey - right.SortKey) is var s and not 0 ? s : 0;
}

/// <summary>
/// Defines a equality comparer used for comparison with two <see cref="ChainingStep"/> instances.
/// </summary>
/// <seealso cref="ChainingStep"/>
file sealed class EqualityComparer : IEqualityComparer<ChainingStep>
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly EqualityComparer Instance = new();


	/// <summary>
	/// Initializes a <see cref="EqualityComparer"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private EqualityComparer()
	{
	}


	/// <inheritdoc/>
	public unsafe bool Equals(ChainingStep? x, ChainingStep? y)
	{
		return (x, y) switch
		{
			(null, null) => true,
			({ ChainsTargets: var targetsX }, { ChainsTargets: var targetsY })
				=> targetsX.CollectionElementEquals(targetsY, &potentialComparison) && branchEquals(targetsX, targetsY),
			_ => false
		};


		static bool potentialComparison(Potential a, Potential b) => a == b;

		static bool branchEquals(Potential[] a, Potential[] b)
		{
			scoped var i1 = a.EnumerateImmutable();
			scoped var i2 = b.EnumerateImmutable();
			while (i1.MoveNext() && i2.MoveNext())
			{
				if (!i1.Current.FullChainPotentials.CollectionElementEquals(i2.Current.FullChainPotentials, &potentialComparison))
				{
					return false;
				}
			}

			return true;
		}
	}

	/// <inheritdoc/>
	public int GetHashCode(ChainingStep obj)
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
}
