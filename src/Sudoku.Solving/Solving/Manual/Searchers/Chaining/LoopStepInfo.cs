namespace Sudoku.Solving.Manual.Chaining;

/// <summary>
/// Provides a usage of <b>(grouped) continuous nice loop</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="XEnabled">Indicates whether the chain is enabled X strong relations.</param>
/// <param name="YEnabled">Indicates whether the chain is enabled Y strong relations.</param>
/// <param name="Target">The destination node that is off.</param>
public sealed record LoopStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	bool XEnabled, bool YEnabled, in Node Target
) : ChainingStepInfo(Conclusions, Views, XEnabled, YEnabled, default, default, default, default)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		(XEnabled && YEnabled ? 5.0M : 4.5M) + (FlatComplexity - 2).GetExtraDifficultyByLength();

	/// <inheritdoc/>
	public override string? Acronym => "CNL";

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		IsXCycle ? Technique.FishyCycle : IsXyChain ? Technique.XyCycle : Technique.ContinuousNiceLoop;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.Aic;

	/// <inheritdoc/>
	public override ChainingTypeCode SortKey => Enum.Parse<ChainingTypeCode>(TechniqueCode.ToString());

	/// <inheritdoc/>
	public override int FlatComplexity => Target.AncestorsCount;

	/// <summary>
	/// Indicates whether the specified cycle is an X-Cycle.
	/// </summary>
	private bool IsXCycle => XEnabled && !YEnabled;

	[FormatItem]
	private string ChainStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new LinkCollection(Views[0].Links!).ToString();
	}
}
