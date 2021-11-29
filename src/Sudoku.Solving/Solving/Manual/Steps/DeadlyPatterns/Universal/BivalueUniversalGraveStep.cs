namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Universal;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public abstract record BivalueUniversalGraveStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views
) : DeadlyPatternStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.6M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.BivalueUniversalGrave;
}
