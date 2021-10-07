namespace Sudoku.Solving.Manual.Exocets;

/// <summary>
/// Provides a usage of <b>junior exocet</b> (JE) technique.
/// </summary>
/// <param name="Views">All views.</param>
/// <param name="Exocet">The exocet.</param>
/// <param name="Digits">All digits.</param>
/// <param name="LockedMemberQ">The locked member Q.</param>
/// <param name="LockedMemberR">The locked member R.</param>
/// <param name="Eliminations">All eliminations.</param>
public sealed record JeStepInfo(
	IReadOnlyList<View> Views, in Pattern Exocet, IEnumerable<int> Digits,
	IEnumerable<int>? LockedMemberQ, IEnumerable<int>? LockedMemberR,
	IReadOnlyList<Elimination> Eliminations
) : ExocetStepInfo(Views, Exocet, Digits, LockedMemberQ, LockedMemberR, Eliminations)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		9.4M + MirrorDifficulty + BiBiDifficulty + TargetPairDifficulty + GeneralizedSwordfishDifficulty;

	/// <inheritdoc/>
	public override string? Acronym => "JE";

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.Je;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.Exocet;

	/// <summary>
	/// Indicates extra difficulty on mirror eliminations.
	/// </summary>
	private decimal MirrorDifficulty =>
		Eliminations.Any(static e => e.Reason == EliminatedReason.Mirror) ? .1M : 0;

	/// <summary>
	/// Indicates extra difficulty on Bi-Bi pattern eliminations.
	/// </summary>
	private decimal BiBiDifficulty =>
		Eliminations.Any(static e => e.Reason == EliminatedReason.BiBiPattern) ? .3M : 0;

	/// <summary>
	/// Indicates extra difficulty on target pair eliminations.
	/// </summary>
	private decimal TargetPairDifficulty =>
		Eliminations.Any(static e => e.Reason == EliminatedReason.TargetPair) ? .1M : 0;

	/// <summary>
	/// Indicates extra difficulty on generalized swordfish eliminations.
	/// </summary>
	private decimal GeneralizedSwordfishDifficulty =>
		Eliminations.Any(static e => e.Reason == EliminatedReason.GeneralizedSwordfish) ? .2M : 0;
}
