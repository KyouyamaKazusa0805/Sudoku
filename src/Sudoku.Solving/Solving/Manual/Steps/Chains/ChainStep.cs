using static Sudoku.Solving.Manual.Steps.IChainStep;

namespace Sudoku.Solving.Manual.Steps.Chains;

/// <summary>
/// Provides with a step that is a <b>Chain</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="XEnabled">
/// <para>A <see cref="bool"/> result indicating whether the chain contains X factor.</para>
/// <para>An <b>X factor</b> represents a chain relation about two same-digit strong link.</para>
/// </param>
/// <param name="YEnabled">
/// <para>A <see cref="bool"/> result indicating whether the chain contains Y factor.</para>
/// <para>A <b>Y factor</b> represents a chain relation about two-digit strong link in a single cell.</para>
/// </param>
/// <param name="IsNishio">
/// Indicates whether the chain is about the technique <b>Nishio Forcing Chains</b>.
/// </param>
/// <param name="IsMultiple">
/// Indicates whether the chain is about the technique <b>Multiple Forcing Chains</b>.
/// </param>
/// <param name="IsDynamic">
/// Indicates whether the chain is about the technique <b>Dynamic Forcing chains</b>.
/// </param>
/// <param name="Level">
/// Indicates the level of the dynamic case. The dynamic level contains 6 possible values, which is corresponding
/// to the digit 0, 1, 2, 3, 4 and 5. For more information, please visit the step searcher to check
/// the property <see cref="IDynamicForcingChainStepSearcher.Level"/>
/// </param>
public abstract record ChainStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	bool XEnabled,
	bool YEnabled,
	bool IsNishio,
	bool IsMultiple,
	bool IsDynamic,
	byte Level
) : Step(Conclusions, Views), IDistinctableStep<ChainStep>, IChainStep
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override bool IsElementary => base.IsElementary;

	/// <inheritdoc/>
	public abstract int FlatComplexity { get; }

	/// <summary>
	/// The total complexity.
	/// </summary>
	public int Complexity => FlatComplexity;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public abstract ChainTypeCode SortKey { get; }

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;

	/// <inheritdoc/>
	public override Technique TechniqueCode => this switch
	{
		{ IsNishio: true } => Technique.NishioFc,
		{ IsDynamic: true } => SortKey switch
		{
			ChainTypeCode.DynamicRegionFc => Technique.DynamicRegionFc,
			ChainTypeCode.DynamicCellFc => Technique.DynamicCellFc,
			ChainTypeCode.DynamicContradictionFc => Technique.DynamicContradictionFc,
			ChainTypeCode.DynamicDoubleFc => Technique.DynamicDoubleFc
		},
		{ IsMultiple: true } => SortKey switch
		{
			ChainTypeCode.RegionFc => Technique.RegionFc,
			ChainTypeCode.CellFc => Technique.CellFc,
		},
		_ => Technique.Aic
	};

	/// <summary>
	/// The base difficulty.
	/// </summary>
	protected decimal BaseDifficulty => this switch
	{
		{ IsNishio: true } => 7.5M,
		{ IsDynamic: true } => Level switch
		{
			0 => 8.5M,
			1 => 8.5M + .5M * Level,
			>= 2 => 9.5M + .5M * (Level - 2)
		},
		{ IsMultiple: true } => 8.0M
	};

	/// <summary>
	/// The length difficulty.
	/// </summary>
	protected decimal LengthDifficulty
	{
		get
		{
			decimal result = 0;
			int ceil = 4;
			int length = Complexity - 2;
			for (bool isOdd = false; length > ceil; isOdd = !isOdd)
			{
				result += .1M;
				ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
			}

			return result;
		}
	}


	/// <inheritdoc/>
	public static unsafe bool Equals(ChainStep left, ChainStep right) => (Left: left, Right: right) switch
	{
		(Left: { SortKey: var lSortKey }, Right: { SortKey: var rSortKey }) when lSortKey != rSortKey => false,
		(
			Left: AlternatingInferenceChainStep { Target: { Root: var lRoot } lTarget, Conclusions: var lc },
			Right: AlternatingInferenceChainStep { Target: { Root: var rRoot } rTarget, Conclusions: var rc }
		) when lTarget == rTarget && lRoot == rRoot && ConclusionsEquals(lc, rc, shouldSort: true) => true,
		(
			Left: ContinuousNiceLoopStep { Target: { Root: var lRoot } lTarget, Conclusions: var lc },
			Right: ContinuousNiceLoopStep { Target: { Root: var rRoot } rTarget, Conclusions: var rc }
		) when lTarget == rTarget && lRoot == rRoot && ConclusionsEquals(lc, rc, shouldSort: true) => true,
		// TODO: Other possible chain types checking.
		_ => false
	};
}
