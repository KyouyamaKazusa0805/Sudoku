namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bidirectional Cycle</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="DestinationOn">Indicates the destination node that is "on" status.</param>
/// <param name="IsX"><inheritdoc/></param>
/// <param name="IsY"><inheritdoc/></param>
internal sealed record SudokuExplainerCompatibleCycleStep(
	ConclusionList Conclusions,
	Potential DestinationOn,
	bool IsX,
	bool IsY
) : SudokuExplainerCompatibleChainStep(Conclusions, IsX, IsY, false, false, false, 0)
{
	/// <inheritdoc/>
	public override int SortKey => (IsX, IsY) switch { (true, true) => 4, (_, true) => 3, _ => 2 };

	/// <inheritdoc/>
	public override int FlatComplexity => AncestorsCountOf(DestinationOn);

	/// <inheritdoc/>
	public override decimal Difficulty => (IsX, IsY) switch { (true, true) => 5.0M, _ => 4.5M } + LengthDifficulty;

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> (IsX, IsY) switch { (true, true) => Technique.ContinuousNiceLoop, (_, true) => Technique.XyChain, _ => Technique.FishyCycle };

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	/// <inheritdoc/>
	protected override int FlatViewsCount => 1;

	/// <inheritdoc/>
	protected override Potential Result => default;


	/// <inheritdoc/>
	protected internal override List<Potential> GetChainsTargets() => new() { DestinationOn };

	/// <inheritdoc/>
	protected override Potential GetChainTargetAt(int viewIndex) => DestinationOn;

	/// <inheritdoc/>
	protected override Candidates GetGreenPotentials(int viewNum) => GetColorCandidates(DestinationOn, true, false);

	/// <inheritdoc/>
	protected override Candidates GetRedPotentials(int viewNum) => GetColorCandidates(DestinationOn, false, true);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex) => GetLinks(DestinationOn);
}
