﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Brute Force</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal sealed record BruteForceStep(ConclusionList Conclusions, ViewList Views) : LastResortStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 20.0M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BruteForce;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.BruteForce;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Always;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { AssignmentStr } }, { "zh", new[] { AssignmentStr } } };

	private string AssignmentStr => ConclusionFormatter.Format(Conclusions.ToArray(), FormattingMode.Normal);
}
