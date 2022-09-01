namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern">Indicates the pattern used.</param>
internal abstract partial record QiuDeadlyPatternStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern
) : DeadlyPatternStep(Conclusions, Views), IStepWithDistinctionDegree
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.8M;

	/// <inheritdoc/>
	public int DistinctionDegree => 1;

	/// <summary>
	/// Indicates the type of the current technique.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode
		=> Type == 5 ? Technique.LockedQiuDeadlyPattern : Enum.Parse<Technique>($"QiuDeadlyPatternType{Type}");

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.DeadlyPattern;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;

	/// <summary>
	/// Indicates the pattern string.
	/// </summary>
	[ResourceTextFormatter]
	private partial string PatternStr() => Pattern.Map.ToString();
}
