using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Guardian</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="Loop">Indicates the loop cells used.</param>
/// <param name="Guardians">Indicates the guardian cells used.</param>
public sealed record GuardianStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Digit,
	in Cells Loop,
	in Cells Guardians
) : SingleDigitPatternStep(Conclusions, Views, Digit),
	IDistinctableStep<GuardianStep>,
	ILoopLikeStep,
	IStepWithRank
{
	/// <inheritdoc/>
	public bool? IsNice => null;

	/// <inheritdoc/>
	public int Rank => -1;

	/// <inheritdoc/>
	public override decimal Difficulty => 5.5M + .1M * (Loop.Count + (Guardians.Count >> 1) >> 1);

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BrokenWing;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.LongChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.BrokenWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Loop.ToString();
	}

	[FormatItem]
	internal string GuardianSingularOrPlural
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => R[Guardians.Count == 1 ? "GuardianSingular" : "GuardianPlural"]!;
	}

	[FormatItem]
	internal string GuardianStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Guardians.ToString();
	}


	/// <inheritdoc/>
	public static bool Equals(GuardianStep left, GuardianStep right) =>
		left.Digit == right.Digit && left.Loop == right.Loop && left.Guardians == right.Guardians;
}
