namespace Sudoku.Solving.Manual.Steps.Exocets;

/// <summary>
/// Provides with a step that is a <b>Junior Exocet</b> technique.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Exocet"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="LockedMemberQ">Indicates the locked member bound with Q cells.</param>
/// <param name="LockedMemberR">Indicates the locked member bound with R cells.</param>
/// <param name="Eliminations"><inheritdoc/></param>
public sealed record JuniorExocetStep(
	ImmutableArray<PresentationData> Views,
	in ExocetPattern Exocet,
	short DigitsMask,
	short LockedMemberQ,
	short LockedMemberR,
	ImmutableArray<ExocetElimination> Eliminations
) : ExocetStep(Views, Exocet, DigitsMask, Eliminations)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		9.4M + MirrorDifficulty + BiBiDifficulty + TargetPairDifficulty + GeneralizedSwordfishDifficulty;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.JuniorExocet;

	/// <summary>
	/// Indicates extra difficulty on mirror eliminations.
	/// </summary>
	private decimal MirrorDifficulty =>
		Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.Mirror) ? .1M : 0;

	/// <summary>
	/// Indicates extra difficulty on Bi-Bi pattern eliminations.
	/// </summary>
	private decimal BiBiDifficulty =>
		Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.BiBiPattern) ? .3M : 0;

	/// <summary>
	/// Indicates extra difficulty on target pair eliminations.
	/// </summary>
	private decimal TargetPairDifficulty =>
		Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.TargetPair) ? .1M : 0;

	/// <summary>
	/// Indicates extra difficulty on generalized swordfish eliminations.
	/// </summary>
	private decimal GeneralizedSwordfishDifficulty =>
		Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.GeneralizedSwordfish) ? .2M : 0;

	/// <summary>
	/// Indicates the locked member Q string.
	/// </summary>
	[FormatItem]
	[NotNullIfNotNull(nameof(LockedMemberQ))]
	private string LockedMemberQStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			string snippet = TextResources.Current.LockedMemberQSnippet;
			string? cells = LockedMemberQ == 0 ? null : new DigitCollection(LockedMemberQ).ToString();
			return $"{snippet}{cells}";
		}
	}

	/// <summary>
	/// Indicates the locked member R string.
	/// </summary>
	[FormatItem]
	[NotNullIfNotNull(nameof(LockedMemberR))]
	private string LockedMemberRStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			string snippet = TextResources.Current.LockedMemberRSnippet;
			string? cells = LockedMemberR == 0 ? null : new DigitCollection(LockedMemberR).ToString();
			return $"{snippet}{cells}";
		}
	}
}
