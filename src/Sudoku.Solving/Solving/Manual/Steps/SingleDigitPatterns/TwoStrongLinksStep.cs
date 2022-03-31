namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="BaseRegion">Indicates the base region used.</param>
/// <param name="TargetRegion">Indicates the target region used.</param>
public sealed record TwoStrongLinksStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<View> Views,
	int Digit,
	int BaseRegion,
	int TargetRegion
) : SingleDigitPatternStep(Conclusions, Views, Digit), IChainLikeStep, IStepWithRank
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		TechniqueCode switch
		{
			Technique.TurbotFish => 4.2M,
			Technique.Skyscraper => 4.0M,
			Technique.TwoStringKite => 4.1M,
			_ => throw new NotSupportedException("The specified technique code is not supported.")
		};

	/// <inheritdoc/>
	public int Rank => 1;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		(BaseRegion / 9, TargetRegion / 9) switch
		{
			(0, _) or (_, 0) => Technique.TurbotFish,
			(1, 1) or (2, 2) => Technique.Skyscraper,
			(1, 2) or (2, 1) => Technique.TwoStringKite,
			_ => throw new InvalidOperationException("The currnet status is invalid.")
		};

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	[FormatItem]
	internal string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	internal string BaseRegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(BaseRegion).ToString();
	}

	[FormatItem]
	internal string TargetRegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(TargetRegion).ToString();
	}
}
