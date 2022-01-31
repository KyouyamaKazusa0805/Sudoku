using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Rank Theory</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public abstract record RankTheoryStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views
) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override bool IsElementary => base.IsElementary;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.RankTheory;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;
}
