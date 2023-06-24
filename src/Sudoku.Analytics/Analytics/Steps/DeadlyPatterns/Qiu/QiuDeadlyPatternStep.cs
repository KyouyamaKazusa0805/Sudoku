namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="pattern">Indicates the pattern.</param>
public abstract partial class QiuDeadlyPatternStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] scoped in QiuDeadlyPattern pattern
) : DeadlyPatternStep(conclusions, views)
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

	private protected string PatternStr => Pattern.Map.ToString();
}
