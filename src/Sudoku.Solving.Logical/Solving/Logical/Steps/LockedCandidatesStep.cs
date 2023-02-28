﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Locked Candidates</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit">Indicates the digit used.</param>
/// <param name="BaseSet">Indicates the house that the current locked candidates forms.</param>
/// <param name="CoverSet">Indicates the house that the current locked candidates influences.</param>
internal sealed record LockedCandidatesStep(ConclusionList Conclusions, ViewList Views, int Digit, int BaseSet, int CoverSet) :
	IntersectionStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => BaseSet < 9 ? 2.6M : 2.8M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => BaseSet < 9 ? Technique.Pointing : Technique.Claiming;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.LockedCandidates;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;


	[ResourceTextFormatter]
	internal string DigitStr() => (Digit + 1).ToString();

	[ResourceTextFormatter]
	internal string BaseSetStr() => HouseFormatter.Format(1 << BaseSet);

	[ResourceTextFormatter]
	internal string CoverSetStr() => HouseFormatter.Format(1 << CoverSet);
}
