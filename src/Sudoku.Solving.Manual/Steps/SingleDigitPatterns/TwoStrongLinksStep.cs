namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="BaseHouse">Indicates the base house used.</param>
/// <param name="TargetHouse">Indicates the target house used.</param>
public sealed record class TwoStrongLinksStep(
	ConclusionList Conclusions, ViewList Views, int Digit, int BaseHouse, int TargetHouse) :
	SingleDigitPatternStep(Conclusions, Views, Digit),
	IChainLikeStep,
	IStepWithRank
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
		(BaseHouse / 9, TargetHouse / 9) switch
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
	internal string BaseHouseStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new HouseCollection(BaseHouse).ToString();
	}

	[FormatItem]
	internal string TargetHouseStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new HouseCollection(TargetHouse).ToString();
	}
}
