namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Chain</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="IsX">Indicates whether the chain is X-Chain.</param>
/// <param name="IsY">Indicates whether the chain is Y-Chain.</param>
/// <param name="IsMultiple"><inheritdoc cref="SudokuExplainerCompatibleChainingStepSearcher.AllowMultiple"/></param>
/// <param name="IsDynamic"><inheritdoc cref="SudokuExplainerCompatibleChainingStepSearcher.AllowDynamic"/></param>
/// <param name="IsNishio"><inheritdoc cref="SudokuExplainerCompatibleChainingStepSearcher.AllowNishio"/></param>
/// <param name="DynamicNestingLevel"><inheritdoc cref="SudokuExplainerCompatibleChainingStepSearcher.DynamicNestingLevel"/></param>
internal abstract partial record SudokuExplainerCompatibleChainStep(
	ConclusionList Conclusions,
	bool IsX,
	bool IsY,
	bool IsMultiple,
	bool IsDynamic,
	bool IsNishio,
	int DynamicNestingLevel
) : ChainStep(Conclusions, ViewList.Empty)
{
	/// <summary>
	/// Indicates the complexity of the chain.
	/// </summary>
	public int Complexity => FlatComplexity + NestedComplexity;

	/// <summary>
	/// Indicates the nested complexity of the chain. This property is useful on checking nesting chains.
	/// </summary>
	public int NestedComplexity
	{
		get
		{
			var result = 0;
			var processed = new HashSet<FullChain>();
			foreach (var target in GetChainsTargets())
			{
				foreach (var p in GetChain(target))
				{
					if (p.NestedChain is { } nested)
					{
						var f = new FullChain(nested);
						if (!processed.Contains(f))
						{
							result += nested.Complexity;

							processed.Add(f);
						}
					}
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates an <see cref="int"/> value indicating the ordering priority of the chain. Greater is heavier.
	/// </summary>
	public abstract int SortKey { get; }

	/// <summary>
	/// Indicates the total number of views the step will be displayed.
	/// </summary>
	public int ViewsCount => FlatViewsCount + NestedViewsCount;

	/// <summary>
	/// Indicates the difficulty rating of the current step, which binds with length factor.
	/// </summary>
	protected decimal LengthDifficulty
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
	/// Gets the base difficulty rating for non-AICs via the settings of the current step.
	/// </summary>
	/// <returns>The base difficulty rating.</returns>
	/// <exception cref="InvalidOperationException">Throws when the current state of the step is invalid.</exception>
	protected decimal BaseDifficultyNonAlternatingInference
		=> this switch
		{
			{ DynamicNestingLevel: var l and >= 2 } => 9.5M + .5M * (l - 2),
			{ DynamicNestingLevel: var l and > 0 } => 8.5M + .5M * l,
			{ IsNishio: true } => 7.5M,
			{ IsDynamic: true } => 8.5M,
			{ IsMultiple: true } => 8.0M,
			_ => throw new InvalidOperationException("The current state of the step searcher is invalid.")
		};

	/// <summary>
	/// Returns how many views the current step will be used.
	/// </summary>
	protected abstract int FlatViewsCount { get; }

	/// <summary>
	/// Returns the number of nested views.
	/// </summary>
	protected int NestedViewsCount
	{
		get
		{
			var result = 0;
			var processed = new HashSet<FullChain>();
			foreach (var target in GetChainsTargets())
			{
				foreach (var p in GetChain(target))
				{
					if (p.NestedChain is not null)
					{
						var f = new FullChain(p.NestedChain);
						if (!processed.Contains(f))
						{
							result += p.NestedChain.ViewsCount;
							processed.Add(f);
						}
					}
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the result node.
	/// </summary>
	protected abstract Potential Result { get; }


	/// <summary>
	/// Try to get all ancestors from the specified node.
	/// </summary>
	/// <param name="child">The specified node.</param>
	/// <returns>The total number of all found ancestors.</returns>
	protected int AncestorsCountOf(Potential child)
	{
		var ancestors = new HashSet<Potential>();
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

	[GeneratedDeconstruction]
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private partial void Deconstruct(out bool isMultiple, out bool isDynamic, out bool isNishio, out int dynamicNestingLevel);

	/// <summary>
	/// Gets the technique name.
	/// </summary>
	/// <returns>The technique name.</returns>
	/// <exception cref="InvalidOperationException">Throws when the state of the current instance is invalid.</exception>
	[ResourceTextFormatter]
	internal string TechniqueName()
	{
		return this switch
		{
			(false, false, false, 0) => R["NormalChains"]!,
			SudokuExplainerCompatibleCellForcingChainsStep(_, false, _, 0) => R["CellChains"]!,
			SudokuExplainerCompatibleCellForcingChainsStep(_, true, _, 0) => $"{dynamicKeyword()}{R["CellChains"]!}",
			SudokuExplainerCompatibleHouseForcingChainsStep(_, false, _, 0) => R["HouseChains"]!,
			SudokuExplainerCompatibleHouseForcingChainsStep(_, true, _, 0) => $"{dynamicKeyword()}{R["HouseChains"]!}",
			SudokuExplainerCompatibleBinaryForcingChainsStep(_, _, true, _) => R["NishioChains"]!,
			SudokuExplainerCompatibleBinaryForcingChainsStep { IsAbsurd: true } => R["AbsurdChains"]!,
			SudokuExplainerCompatibleBinaryForcingChainsStep => R["DoubleChains"]!,
			(_, _, _, var l and not 0) => $"{R[""]!}{nestedSuffix(l)}",
			_ => throw new InvalidOperationException("The status of the current instance is invalid.")
		};


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
		foreach (var target in GetChainsTargets())
		{
			// Iterate on chain targets.
			CollectRuleParents(initialGrid, currentGrid, result, target);
		}

		return result;
	}

	/// <summary>
	/// Try to get all possible chains targets.
	/// </summary>
	/// <returns>All <see cref="Potential"/> instances.</returns>
	protected internal abstract List<Potential> GetChainsTargets();

	/// <summary>
	/// Gets the chain of all <see cref="Potential"/>s from the specified target.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <returns>The chain of all <see cref="Potential"/>s</returns>
	protected internal List<Potential> GetChain(Potential target)
	{
		var result = new List<Potential>();
		var done = new HashSet<Potential>();
		var todo = new List<Potential> { target };
		while (todo.Count > 0)
		{
			var next = new List<Potential>();
			foreach (var p in todo)
			{
				if (!done.Contains(p))
				{
					done.Add(p);
					result.Add(p);
					next.AddRange(p.Parents);
				}
			}

			todo = next;
		}

		return result;
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
	/// Try to get the target node of a chain, displayed in the view at the specified index.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	/// <returns>The <see cref="Potential"/> result.</returns>
	protected abstract Potential GetChainTargetAt(int viewIndex);

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
		foreach (var p in GetChain(target))
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
			foreach (var (cell, digit, isOn) in GetChain(target))
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

			foreach (var p in blues)
			{
				result.Add(p.Cell * 9 + p.Digit);
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
		foreach (var p in GetChain(target))
		{
			if (p.Parents.Count <= 6)
			{
				foreach (var pr in p.Parents)
				{
					result.Add(new(DisplayColorKind.Normal, new(pr.Digit, pr.Cell), new(p.Digit, p.Cell), Inference.Default));
				}
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
	/// Try to get container target potential via the specified step.
	/// </summary>
	/// <param name="step">The step instance.</param>
	/// <returns>The container target.</returns>
	private Potential GetContainerTarget(SudokuExplainerCompatibleChainStep step)
	{
		foreach (var target in GetChainsTargets())
		{
			foreach (var p in GetChain(target))
			{
				if (p.NestedChain == step)
				{
					return p;
				}
			}
		}

		return default;
	}

	/// <summary>
	/// Try to get nested chains, represented as <see cref="SudokuExplainerCompatibleChainStep"/> instances.
	/// </summary>
	/// <returns>A list of <see cref="SudokuExplainerCompatibleChainStep"/> instances.</returns>
	private IEnumerable<SudokuExplainerCompatibleChainStep> GetNestedChains()
	{
		var result = new List<SudokuExplainerCompatibleChainStep>();
		var processed = new HashSet<FullChain>();
		foreach (var target in GetChainsTargets())
		{
			foreach (var p in GetChain(target))
			{
				if (p.NestedChain is not null)
				{
					var f = new FullChain(p.NestedChain);
					if (!processed.Contains(f))
					{
						result.Add(p.NestedChain);
						processed.Add(f);
					}
				}
			}
		}

		// Recurse (in case there is more than one level of nesting).
		foreach (var chain in new List<SudokuExplainerCompatibleChainStep>(result))
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
	/// <description>A <see cref="SudokuExplainerCompatibleChainStep"/> instance.</description>
	/// </item>
	/// <item>
	/// <term>Second</term>
	/// <description>An index of the step to be displayed.</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <param name="nestedViewIndex">The nested view index.</param>
	/// <returns>A pair of values.</returns>
	private (SudokuExplainerCompatibleChainStep Step, int ViewIndex) GetNestedChain(int nestedViewIndex)
	{
		var processed = new HashSet<FullChain>();
		foreach (var target in GetChainsTargets())
		{
			foreach (var p in GetChain(target))
			{
				if (p.NestedChain is not null)
				{
					var f = new FullChain(p.NestedChain);
					if (!processed.Contains(f))
					{
						processed.Add(f);
						var localCount = p.NestedChain.ViewsCount;
						if (localCount > nestedViewIndex)
						{
							return (p.NestedChain, nestedViewIndex);
						}

						nestedViewIndex -= localCount;
					}
				}
			}
		}

		return default;
	}


	/// <summary>
	/// Compares two <see cref="SudokuExplainerCompatibleChainStep"/> instance, and returns an <see cref="int"/>
	/// indicating which value is greater.
	/// </summary>
	/// <param name="left">The left-side value to be compared.</param>
	/// <param name="right">The right-side value to be compared.</param>
	/// <returns>An <see cref="int"/> value indicating which is greater.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compare(SudokuExplainerCompatibleChainStep left, SudokuExplainerCompatibleChainStep right)
		=> (left, right) switch
		{
			({ Difficulty: var d1 }, { Difficulty: var d2 }) when d1 != d2 => Sign(d1 - d2),
			({ Complexity: var l1 }, { Complexity: var l2 }) when l1 != l2 => Sign(l1 - l2),
			({ SortKey: var s1 }, { SortKey: var s2 }) => Sign(s1 - s2)
		};
}
