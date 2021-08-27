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
/// to the digit 0, 1, 2, 3, 4 and 5.
/// <list type="table">
/// <item>
/// <term>Level 0</term>
/// <description>The dynamic case is disabled. The chain isn't a dynamic chain at all.</description>
/// </item>
/// <item>
/// <term>Level 1</term>
/// <description>The chain is a normal dynamic forcing chains.</description>
/// </item>
/// <item>
/// <term>Level 2</term>
/// <description>
/// The chain is a dynamic forcing chains with grouped nodes, such as an X-Wing.
/// (i.e. <b>Dynamic Forcing Chains + Generalized Structure</b>)
/// </description>
/// </item>
/// <item>
/// <term>Level 3</term>
/// <description>
/// The chain is a dynamic forcing chains with normal AICs as grouped nodes.
/// (i.e. <b>Dynamic Forcing Chains + Alternating Inference Chain</b>)
/// </description>
/// </item>
/// <item>
/// <term>Level 4</term>
/// <description>
/// The chain is a dynamic forcing chains with normal forcing chains as grouped nodes.
/// (i.e. <b>Dynamic Forcing Chains + Forcing Chains</b>)
/// </description>
/// </item>
/// <item>
/// <term>Level 5</term>
/// <description>
/// The chain is a dynamic forcing chains with dynamic forcing chains as grouped nodes.
/// (i.e. <b>Dynamic Forcing Chains + Dynamic Forcing Chains</b>)
/// </description>
/// </item>
/// </list>
/// </param>
public abstract record ChainStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
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

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public abstract ChainTypeCode SortKey { get; }

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;


	/// <inheritdoc/>
	public static unsafe bool Equals(ChainStep left, ChainStep right) => (Left: left, Right: right) switch
	{
		(Left: { SortKey: var lSortKey }, Right: { SortKey: var rSortKey }) when lSortKey != rSortKey => false,
		(
			Left: AlternatingInferenceChainStep { Target: { Root: var lRoot } lTarget, Conclusions: var lc },
			Right: AlternatingInferenceChainStep { Target: { Root: var rRoot } rTarget, Conclusions: var rc }
		) when lTarget == rTarget && *lRoot == *rRoot && ConclusionsEquals(lc, rc, shouldSort: true) => true,
		(
			Left: ContinuousNiceLoopStep { Target: { Root: var lRoot } lTarget, Conclusions: var lc },
			Right: ContinuousNiceLoopStep { Target: { Root: var rRoot } rTarget, Conclusions: var rc }
		) when lTarget == rTarget && *lRoot == *rRoot && ConclusionsEquals(lc, rc, shouldSort: true) => true,
		// TODO: Other possible chain types checking.
		_ => false
	};
}
