namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Rank Theory</b> technique.
/// </summary>
public abstract class RankTheoryStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	[MaybeNull]
	[AllowNull]
	public sealed override string Format => base.Format;
}
