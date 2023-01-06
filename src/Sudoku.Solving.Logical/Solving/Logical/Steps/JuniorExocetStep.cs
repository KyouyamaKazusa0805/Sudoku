namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Junior Exocet</b> technique.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Exocet"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="Eliminations"><inheritdoc/></param>
#if false
/// <param name="LockedMemberQ">Indicates the locked member bound with Q cells.</param>
/// <param name="LockedMemberR">Indicates the locked member bound with R cells.</param>
#endif
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record JuniorExocetStep(
	ViewList Views,
	scoped in Exocet Exocet,
	short DigitsMask,
#if false
	short LockedMemberQ,
	short LockedMemberR,
#endif
	ImmutableArray<ExocetElimination> Eliminations
) : ExocetStep(Views, Exocet, DigitsMask, Eliminations), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 9.4M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			(
				PhasedDifficultyRatingKinds.Mirror,
				Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.Mirror) ? .1M : 0
			),
			(
				PhasedDifficultyRatingKinds.BiBiPattern,
				Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.BiBiPattern) ? .3M : 0
			),
			(
				PhasedDifficultyRatingKinds.TargetPair,
				Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.TargetPair) ? .1M : 0
			),
			(
				PhasedDifficultyRatingKinds.GeneralizedSwordfish,
				Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.GeneralizedSwordfish) ? .2M : 0
			)
		};

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.JuniorExocet;


	/// <summary>
	/// Indicates the locked member Q string.
	/// </summary>
	[ResourceTextFormatter]
	internal string LockedMemberQStr()
	{
#if false
		string? cells = LockedMemberQ == 0 ? null : DigitMaskFormatter.Format(LockedMemberQ, FormattingMode.Normal);
		return $"{R["LockedMember1"]}{cells}";
#else
		return string.Empty;
#endif
	}

	/// <summary>
	/// Indicates the locked member R string.
	/// </summary>
	[ResourceTextFormatter]
	internal string LockedMemberRStr()
	{
#if false
		string? cells = LockedMemberR == 0 ? null : DigitMaskFormatter.Format(LockedMemberR, FormattingMode.Normal);
		return $"{R["LockedMember2"]}{cells}";
#else
		return string.Empty;
#endif
	}
}
