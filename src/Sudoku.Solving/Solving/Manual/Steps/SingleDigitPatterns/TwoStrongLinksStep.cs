using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

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
	ImmutableArray<PresentationData> Views,
	int Digit,
	int BaseRegion,
	int TargetRegion
) : SingleDigitPatternStep(Conclusions, Views, Digit)
{
	/// <inheritdoc/>
	public override decimal Difficulty => TechniqueCode switch
	{
		Technique.TurbotFish => 4.2M,
		Technique.Skyscraper => 4.0M,
		Technique.TwoStringKite => 4.1M
	};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override Technique TechniqueCode => (BaseKind: BaseRegion / 9, TargetKind: TargetRegion / 9) switch
	{
		(BaseKind: 0, _) or (_, TargetKind: 0) => Technique.TurbotFish,
		(BaseKind: 1, TargetKind: 1) or (BaseKind: 2, TargetKind: 2) => Technique.Skyscraper,
		(BaseKind: 1, TargetKind: 2) or (BaseKind: 2, TargetKind: 1) => Technique.TwoStringKite
	};

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	private string BaseRegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(BaseRegion).ToString();
	}

	[FormatItem]
	private string TargetRegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(TargetRegion).ToString();
	}
}
