namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class BivalueUniversalGraveStep(Conclusion[] conclusions, View[]? views) : DeadlyPatternStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.6M;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public abstract override Technique Code { get; }
}
