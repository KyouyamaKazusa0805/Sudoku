namespace Sudoku.Solving.Manual.Alses;

/// <summary>
/// Provides a usage of <b>almost locked sets W-Wing</b> (ALS-W-Wing) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Als1">The ALS 1.</param>
/// <param name="Als2">The ALS 2.</param>
/// <param name="ConjugatePair">The conjugate pair.</param>
/// <param name="WDigitsMask">The W digit mask.</param>
/// <param name="X">The digit X.</param>
public sealed record AlsWWingStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Als Als1, in Als Als2,
	in ConjugatePair ConjugatePair, short WDigitsMask, int X
) : AlsStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 6.2M;

	/// <inheritdoc/>
	public override string? Acronym => "ALS-W-Wing";

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlsChainingLike;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.AlsWWing;

	[FormatItem]
	private string Als1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Als1.ToString();
	}

	[FormatItem]
	private string Als2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Als2.ToString();
	}

	[FormatItem]
	private string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}

	[FormatItem]
	private string WStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(WDigitsMask).ToString();
	}

	[FormatItem]
	private string XStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (X + 1).ToString();
	}
}
