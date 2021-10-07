namespace Sudoku.Solving.Manual.Sdps;

/// <summary>
/// Provides a usage of <b>guardian</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Digit">The digit used.</param>
/// <param name="Loop">The loop.</param>
/// <param name="Guardians">All guardians.</param>
[AutoHashCode(nameof(Digit), nameof(Loop), nameof(Guardians))]
public sealed partial record GuardianStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	int Digit, in Cells Loop, in Cells Guardians
) : SdpStepInfo(Conclusions, Views, Digit)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.5M + .1M * (Loop.Count + (Guardians.Count >> 1) >> 1);

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.Guardian;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.LongChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.Guardian;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Loop.ToString();
	}

	[FormatItem]
	private string GuardianSingularOrPlural
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Guardians.Count == 1 ? TextResources.Current.GuardianSingular : TextResources.Current.GuardianPlural;
	}

	[FormatItem]
	private string GuardianStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Guardians.ToString();
	}


	/// <inheritdoc/>
	public bool Equals(GuardianStepInfo? other) =>
		other is not null && Loop == other.Loop && Guardians == other.Guardians && Digit == other.Digit;
}
