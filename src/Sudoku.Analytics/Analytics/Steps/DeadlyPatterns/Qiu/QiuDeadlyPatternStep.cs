namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern</b> technique.
/// </summary>
public abstract class QiuDeadlyPatternStep(Conclusion[] conclusions, View[]? views, scoped in QiuDeadlyPattern pattern) :
	DeadlyPatternStep(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 5.8M;

	/// <summary>
	/// Indicates the type of the current technique.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override Technique Code => Type == 5 ? Technique.LockedQiuDeadlyPattern : Enum.Parse<Technique>($"QiuDeadlyPatternType{Type}");

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <summary>
	/// Indicates the pattern.
	/// </summary>
	public QiuDeadlyPattern Pattern { get; } = pattern;

	private protected string PatternStr => Pattern.Map.ToString();
}
