namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record BivalueUniversalGraveStep(ConclusionList Conclusions, ViewList Views) : DeadlyPatternStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.6M;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public abstract override Technique TechniqueCode { get; }

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.BivalueUniversalGrave;
}
